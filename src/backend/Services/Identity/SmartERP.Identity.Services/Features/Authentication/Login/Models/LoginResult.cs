using SmartERP.Identity.Services.Features.Authentication.Login.DTO;

namespace SmartERP.Identity.Services.Features.Authentication.Login.Models;

public sealed record LoginResult(
    LoginResponseDto Response,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
