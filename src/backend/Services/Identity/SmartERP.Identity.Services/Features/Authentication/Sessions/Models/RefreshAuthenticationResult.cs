using SmartERP.Identity.Services.Features.Authentication.Sessions.DTO;

namespace SmartERP.Identity.Services.Features.Authentication.Sessions.Models;

public sealed record RefreshAuthenticationResult(
    RefreshResponseDto Response,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
