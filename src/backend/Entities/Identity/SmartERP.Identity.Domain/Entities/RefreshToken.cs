namespace SmartERP.Identity.Domain.Entities;

public sealed class RefreshToken
{
    public long Id { get; set; }

    public long UserSessionId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset ExpiresAtUtc { get; set; }

    public DateTimeOffset? UsedAtUtc { get; set; }

    public DateTimeOffset? RevokedAtUtc { get; set; }

    public string? RevokedReason { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public string? CreatedByIpAddress { get; set; }

    public string? RevokedByIpAddress { get; set; }

    public UserSession UserSession { get; set; } = null!;

    public bool IsActive =>UsedAtUtc is null &&RevokedAtUtc is null &&ExpiresAtUtc > DateTimeOffset.UtcNow;
}