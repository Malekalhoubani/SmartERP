namespace SmartERP.Identity.Services.Features.Authentication.Sessions.DTO;

public sealed record RefreshResponseDto(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc);
