namespace SmartERP.Identity.Services.Features.Authentication.Sessions.DTO;

public sealed record SessionResponseDto(
    long SessionId,
    string? IpAddress,
    string? UserAgent,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset LastActivityAtUtc,
    DateTimeOffset ExpiresAtUtc,
    bool IsCurrent);
