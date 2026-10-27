using SmartERP.Identity.Services.Features.Authentication.ResetPassword.DTO;

namespace SmartERP.Identity.Services.Interfaces.Authentication;

public interface IResetPasswordService
{
    Task<ResetPasswordResponseDto> ResetPasswordAsync(
        ResetPasswordRequestDto requestDto,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
