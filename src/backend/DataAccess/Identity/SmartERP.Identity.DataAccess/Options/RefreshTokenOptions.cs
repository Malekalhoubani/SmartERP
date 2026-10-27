namespace SmartERP.Identity.DataAccess.Options;

public sealed class RefreshTokenOptions
{
    public const string SectionName = "RefreshToken";

    public int ExpirationDays { get; init; } = 30;

    public int TokenSizeInBytes { get; init; } = 64;

    public int MaxActiveSessionsPerUser { get; init; } = 5;

    public string CookieName { get; init; } = "SmartERP.RefreshToken";
}
