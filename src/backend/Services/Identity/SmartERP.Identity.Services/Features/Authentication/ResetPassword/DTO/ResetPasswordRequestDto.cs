namespace SmartERP.Identity.Services.Features.Authentication.ResetPassword.DTO;

public sealed record ResetPasswordRequestDto(
    int UserId,
    string Token,
    string NewPassword,
    string ConfirmPassword);
