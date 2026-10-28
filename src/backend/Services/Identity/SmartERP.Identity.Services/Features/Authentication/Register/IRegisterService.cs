using SmartERP.Identity.Services.Features.Authentication.Register.DTO;

namespace SmartERP.Identity.Services.Features.Authentication.Register;

public interface IRegisterService
{
    Task<RegisterResponseDto> RegisterAsync(
        RegisterRequestDto requestDto,
        string confirmationBaseUrl,
        CancellationToken cancellationToken = default);

    Task ConfirmEmailAsync(
        int userId,
        string token,
        CancellationToken cancellationToken = default);

    Task<ResendConfirmationEmailResponseDto> ResendConfirmationEmailAsync(
        ResendConfirmationEmailRequestDto requestDto,
        string confirmationBaseUrl,
        CancellationToken cancellationToken = default);
}
