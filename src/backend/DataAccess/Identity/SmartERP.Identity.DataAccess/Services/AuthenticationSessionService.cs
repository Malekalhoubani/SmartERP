using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SmartERP.Identity.DataAccess.Options;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.Sessions;
using SmartERP.Identity.Services.Features.Authentication.Sessions.DTO;
using SmartERP.Identity.Services.Features.Authentication.Sessions.Models;
using SmartERP.Identity.Services.Interfaces;

namespace SmartERP.Identity.DataAccess.Services;

public sealed class AuthenticationSessionService(
    IAuthenticationSessionRepository sessionRepository,
    IIdentityRepository identityRepository,
    IRolePermissionRepository rolePermissionRepository,
    ITokenService tokenService,
    IOptions<RefreshTokenOptions> options)
    : IAuthenticationSessionService
{
    private readonly RefreshTokenOptions _options = options.Value;

    public async Task<CreatedSessionResult> CreateSessionAsync(
        ApplicationUser user,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTimeOffset.UtcNow;
        var expiresAtUtc = utcNow.AddDays(_options.ExpirationDays);
        var tokenValue = CreateTokenValue();

        var session = new UserSession
        {
            UserId = user.Id,
            CreatedAtUtc = utcNow,
            LastActivityAtUtc = utcNow,
            ExpiresAtUtc = expiresAtUtc,
            IpAddress = Normalize(ipAddress, 45),
            UserAgent = Normalize(userAgent, 512),
            RefreshTokens =
            [
                new RefreshToken
                {
                    TokenHash = tokenValue.TokenHash,
                    CreatedAtUtc = utcNow,
                    ExpiresAtUtc = expiresAtUtc,
                    CreatedByIpAddress = Normalize(ipAddress, 45)
                }
            ]
        };

        await sessionRepository.AddAsync(
            session,
            _options.MaxActiveSessionsPerUser,
            cancellationToken);

        return new CreatedSessionResult(
            session.Id,
            tokenValue.Token,
            expiresAtUtc);
    }

    public async Task<RefreshAuthenticationResult> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeHash(refreshToken);

        var storedToken = await sessionRepository.FindRefreshTokenByHashAsync(
            tokenHash,
            cancellationToken);

        if (storedToken is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid refresh token.");
        }

        var utcNow = DateTimeOffset.UtcNow;
        var session = storedToken.UserSession;

        if (storedToken.UsedAtUtc is not null ||
            storedToken.RevokedAtUtc is not null)
        {
            await RevokeSessionAsync(
                session.UserId,
                session.Id,
                "Refresh token reuse detected.",
                ipAddress,
                cancellationToken);

            throw new UnauthorizedAccessException(
                "Refresh token reuse detected. The session was revoked.");
        }

        if (storedToken.ExpiresAtUtc <= utcNow ||
            session.ExpiresAtUtc <= utcNow ||
            session.RevokedAtUtc is not null ||
            !session.User.IsActive)
        {
            await RevokeSessionAsync(
                session.UserId,
                session.Id,
                "Session expired or unavailable.",
                ipAddress,
                cancellationToken);

            throw new UnauthorizedAccessException(
                "The session is no longer active.");
        }

        var newTokenValue = CreateTokenValue();

        storedToken.UsedAtUtc = utcNow;
        storedToken.ReplacedByTokenHash = newTokenValue.TokenHash;

        session.LastActivityAtUtc = utcNow;
        session.IpAddress = Normalize(ipAddress, 45) ?? session.IpAddress;
        session.RefreshTokens.Add(
            new RefreshToken
            {
                TokenHash = newTokenValue.TokenHash,
                CreatedAtUtc = utcNow,
                ExpiresAtUtc = session.ExpiresAtUtc,
                CreatedByIpAddress = Normalize(ipAddress, 45)
            });

        var roles = await identityRepository.GetRolesAsync(
            session.User,
            cancellationToken);

        var permissions =
            await rolePermissionRepository.GetUserPermissionsAsync(
                session.UserId,
                cancellationToken);

        var accessToken = tokenService.GenerateAccessToken(
            session.User,
            roles,
            permissions,
            session.Id);

        await sessionRepository.SaveChangesAsync(cancellationToken);

        return new RefreshAuthenticationResult(
            new RefreshResponseDto(
                accessToken.Token,
                accessToken.Expiration),
            newTokenValue.Token,
            session.ExpiresAtUtc);
    }

    public async Task<IReadOnlyCollection<SessionResponseDto>> GetSessionsAsync(
        int userId,
        long currentSessionId,
        CancellationToken cancellationToken = default)
    {
        var sessions = await sessionRepository.GetActiveSessionsAsync(
            userId,
            cancellationToken);

        return sessions
            .Select(session => new SessionResponseDto(
                session.Id,
                session.IpAddress,
                session.UserAgent,
                session.CreatedAtUtc,
                session.LastActivityAtUtc,
                session.ExpiresAtUtc,
                session.Id == currentSessionId))
            .ToArray();
    }

    public async Task RevokeSessionAsync(
        int userId,
        long sessionId,
        string reason,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var session = await sessionRepository.FindSessionAsync(
            sessionId,
            userId,
            cancellationToken);

        if (session is null)
        {
            throw new KeyNotFoundException("Session was not found.");
        }

        if (session.RevokedAtUtc is not null)
        {
            return;
        }

        RevokeSession(session, reason, ipAddress);
        await sessionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeByRefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await sessionRepository.FindRefreshTokenByHashAsync(
            ComputeHash(refreshToken),
            cancellationToken);

        if (storedToken is null)
        {
            return;
        }

        await RevokeSessionAsync(
            storedToken.UserSession.UserId,
            storedToken.UserSession.Id,
            "User logged out.",
            ipAddress,
            cancellationToken);
    }

    public Task RevokeAllSessionsAsync(
        int userId,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        return sessionRepository.RevokeAllSessionsAsync(
            userId,
            "User logged out from all sessions.",
            Normalize(ipAddress, 45),
            cancellationToken);
    }

    private (string Token, string TokenHash) CreateTokenValue()
    {
        var token = WebEncoders.Base64UrlEncode(
            RandomNumberGenerator.GetBytes(_options.TokenSizeInBytes));

        return (token, ComputeHash(token));
    }

    private static string ComputeHash(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException(
                "Refresh token is required.");
        }

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

    private static void RevokeSession(
        UserSession session,
        string reason,
        string? ipAddress)
    {
        var utcNow = DateTimeOffset.UtcNow;

        session.RevokedAtUtc = utcNow;
        session.RevokedReason = reason;

        foreach (var token in session.RefreshTokens.Where(
                     refreshToken => refreshToken.RevokedAtUtc is null))
        {
            token.RevokedAtUtc = utcNow;
            token.RevokedReason = reason;
            token.RevokedByIpAddress = Normalize(ipAddress, 45);
        }
    }

    private static string? Normalize(string? value, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalizedValue = value.Trim();

        return normalizedValue.Length <= maximumLength
            ? normalizedValue
            : normalizedValue[..maximumLength];
    }
}
