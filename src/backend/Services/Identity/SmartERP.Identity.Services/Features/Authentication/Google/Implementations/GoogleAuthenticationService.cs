using System.Security.Claims;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.Login.DTO;
using SmartERP.Identity.Services.Features.Authentication.Login.Models;
using SmartERP.Identity.Services.Features.Authentication.Sessions;
using SmartERP.Identity.Services.Interfaces;
using SmartERP.Identity.Services.Interfaces.Authentication;
using SmartERP.Identity.Services.Authorization;

namespace SmartERP.Identity.Services.Features.Authentication.Google.Implementations;

public sealed class GoogleAuthenticationService(
    IIdentityRepository identityRepository,
    IRolePermissionRepository rolePermissionRepository,
    IAuthenticationSessionService sessionService,
    ITokenService tokenService)
    : IGoogleAuthenticationService
{
    private const string LoginProvider = "Google";

    public async Task<LoginResult> AuthenticateAsync(
        ClaimsPrincipal externalPrincipal,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var providerKey = externalPrincipal.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var email = externalPrincipal.FindFirstValue(ClaimTypes.Email);
        var fullName = externalPrincipal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrWhiteSpace(providerKey) ||
            string.IsNullOrWhiteSpace(email))
        {
            throw new UnauthorizedAccessException(
                "Google did not provide the required account information.");
        }

        if (!string.Equals(
                externalPrincipal.FindFirstValue(
                    "google_email_verified"),
                "true",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                "Google has not verified this email address.");
        }

        var user = await identityRepository.FindByExternalLoginAsync(
            LoginProvider,
            providerKey,
            cancellationToken);

        if (user is null)
        {
            user = await identityRepository.FindByEmailAsync(
                email,
                cancellationToken);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    Email = email.Trim(),
                    UserName = email.Trim(),
                    FullName = string.IsNullOrWhiteSpace(fullName)
                        ? email.Split('@')[0]
                        : fullName.Trim(),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAtUtc = DateTimeOffset.UtcNow
                };

                var createResult =
                    await identityRepository.CreateExternalUserAsync(
                        user,
                        LoginProvider,
                        providerKey,
                        SystemRoleNames.User,
                        cancellationToken);

                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(" ", createResult.Errors));
                }
            }
            else
            {
                var linkResult =
                    await identityRepository.AddExternalLoginAsync(
                        user,
                        LoginProvider,
                        providerKey,
                        cancellationToken);

                if (!linkResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(" ", linkResult.Errors));
                }

                if (!user.EmailConfirmed)
                {
                    await identityRepository.MarkEmailConfirmedAsync(
                        user,
                        cancellationToken);
                }
            }
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "This account is not active.");
        }

        var roles = await identityRepository.GetRolesAsync(
            user,
            cancellationToken);

        var permissions =
            await rolePermissionRepository.GetUserPermissionsAsync(
                user.Id,
                cancellationToken);

        var session = await sessionService.CreateSessionAsync(
            user,
            ipAddress,
            userAgent,
            cancellationToken);

        var accessToken = tokenService.GenerateAccessToken(
            user,
            roles,
            permissions,
            session.SessionId);

        return new LoginResult(
            new LoginResponseDto(
                user.Id,
                user.FullName,
                user.Email!,
                accessToken.Token,
                accessToken.Expiration),
            session.RefreshToken,
            session.RefreshTokenExpiresAtUtc);
    }
}
