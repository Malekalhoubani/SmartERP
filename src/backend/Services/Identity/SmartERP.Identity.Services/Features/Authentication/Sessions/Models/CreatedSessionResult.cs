namespace SmartERP.Identity.Services.Features.Authentication.Sessions.Models;

public sealed record CreatedSessionResult(
    long SessionId,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
