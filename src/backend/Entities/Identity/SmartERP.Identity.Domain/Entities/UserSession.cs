namespace SmartERP.Identity.Domain.Entities;

public sealed class UserSession
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset LastActivityAtUtc { get; set; }

    public DateTimeOffset ExpiresAtUtc { get; set; }

    public DateTimeOffset? RevokedAtUtc { get; set; }

    public string? RevokedReason { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public ICollection<RefreshToken> RefreshTokens { get; set; } =new List<RefreshToken>();

    public bool IsActive =>RevokedAtUtc is null && ExpiresAtUtc > DateTimeOffset.UtcNow;
}