using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Services.Features.Authentication.Sessions.DTO;
using SmartERP.Identity.Services.Features.Authentication.Sessions.Models;

namespace SmartERP.Identity.Services.Features.Authentication.Sessions;

public interface IAuthenticationSessionService
{
    Task<CreatedSessionResult> CreateSessionAsync(
        ApplicationUser user,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<RefreshAuthenticationResult> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SessionResponseDto>> GetSessionsAsync(
        int userId,
        long currentSessionId,
        CancellationToken cancellationToken = default);

    Task RevokeSessionAsync(
        int userId,
        long sessionId,
        string reason,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task RevokeByRefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task RevokeAllSessionsAsync(
        int userId,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task RevokeAllSessionsAsync(
        int userId,
        string reason,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
