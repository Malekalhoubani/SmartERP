using Microsoft.EntityFrameworkCore;
using SmartERP.Identity.DataAccess.Contexts;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;

namespace SmartERP.Identity.DataAccess.Repositories;

public sealed class AuthenticationSessionRepository(
    IdentityApplicationDbContext dbContext)
    : IAuthenticationSessionRepository
{
    public async Task AddAsync(
        UserSession session,
        int maxActiveSessions,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTimeOffset.UtcNow;

        var activeSessions = await dbContext.UserSessions
            .Include(existingSession => existingSession.RefreshTokens)
            .Where(existingSession =>
                existingSession.UserId == session.UserId &&
                existingSession.RevokedAtUtc == null &&
                existingSession.ExpiresAtUtc > utcNow)
            .OrderBy(existingSession => existingSession.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var sessionsToRevoke = Math.Max(
            0,
            activeSessions.Count - maxActiveSessions + 1);

        foreach (var oldSession in activeSessions.Take(sessionsToRevoke))
        {
            RevokeSession(
                oldSession,
                utcNow,
                "Maximum active sessions reached.",
                session.IpAddress);
        }

        await dbContext.UserSessions.AddAsync(
            session,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> FindRefreshTokenByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return dbContext.RefreshTokens
            .Include(refreshToken => refreshToken.UserSession)
                .ThenInclude(session => session.User)
            .SingleOrDefaultAsync(
                refreshToken => refreshToken.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserSession>> GetActiveSessionsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTimeOffset.UtcNow;

        return await dbContext.UserSessions
            .AsNoTracking()
            .Where(session =>
                session.UserId == userId &&
                session.RevokedAtUtc == null &&
                session.ExpiresAtUtc > utcNow)
            .OrderByDescending(session => session.LastActivityAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public Task<UserSession?> FindSessionAsync(
        long sessionId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.UserSessions
            .Include(session => session.RefreshTokens)
            .SingleOrDefaultAsync(
                session =>
                    session.Id == sessionId &&
                    session.UserId == userId,
                cancellationToken);
    }

    public async Task RevokeAllSessionsAsync(
        int userId,
        string reason,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTimeOffset.UtcNow;

        var sessions = await dbContext.UserSessions
            .Include(session => session.RefreshTokens)
            .Where(session =>
                session.UserId == userId &&
                session.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            RevokeSession(session, utcNow, reason, ipAddress);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void RevokeSession(
        UserSession session,
        DateTimeOffset revokedAtUtc,
        string reason,
        string? ipAddress)
    {
        session.RevokedAtUtc = revokedAtUtc;
        session.RevokedReason = reason;

        foreach (var refreshToken in session.RefreshTokens.Where(
                     token => token.RevokedAtUtc is null))
        {
            refreshToken.RevokedAtUtc = revokedAtUtc;
            refreshToken.RevokedReason = reason;
            refreshToken.RevokedByIpAddress = ipAddress;
        }
    }
}
