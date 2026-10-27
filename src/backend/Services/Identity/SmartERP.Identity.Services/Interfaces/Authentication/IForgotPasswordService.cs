using SmartERP.Identity.Services.Features.Authentication
    .ForgotPassword.DTO;

namespace SmartERP.Identity.Services.Interfaces.Authentication;

public interface IForgotPasswordService
{
    Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto requestDto,string frontendOrigin,CancellationToken cancellationToken = default);
}