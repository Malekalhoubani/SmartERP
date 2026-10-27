using SmartERP.Identity.Domain.Entities;

namespace SmartERP.Identity.Repositories.Interfaces;

public interface IAuthenticationSessionRepository
{
    Task AddAsync(
        UserSession session,
        int maxActiveSessions,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> FindRefreshTokenByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserSession>> GetActiveSessionsAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<UserSession?> FindSessionAsync(
        long sessionId,
        int userId,
        CancellationToken cancellationToken = default);

    Task RevokeAllSessionsAsync(
        int userId,
        string reason,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
