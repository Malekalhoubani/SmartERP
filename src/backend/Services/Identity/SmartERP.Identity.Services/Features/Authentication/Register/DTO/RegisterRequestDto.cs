namespace SmartERP.Identity.Services.Features.Authentication.Register.DTO;

public sealed record RegisterRequestDto(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword);
