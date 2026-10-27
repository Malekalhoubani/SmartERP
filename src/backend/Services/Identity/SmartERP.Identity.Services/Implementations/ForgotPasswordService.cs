using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.ForgotPassword.DTO;
using SmartERP.Identity.Services.Interfaces;
using SmartERP.Identity.Services.Interfaces.Authentication;
using System.Text;
using System.Net;

namespace SmartERP.Identity.Services.Features.Authentication
    .ForgotPassword.Implementations;

public sealed class ForgotPasswordService(
    IIdentityRepository identityRepository,
    IEmailService emailService)
    : IForgotPasswordService
{
    private const string ResponseMessage =
        "If the email exists, a password reset link has been sent.";

    public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto requestDto,string frontendOrigin,CancellationToken cancellationToken = default)
    {
        var email = requestDto.Email.Trim();

        var user = await identityRepository.FindByEmailAsync(
            email,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            return CreateResponse();
        }

        var token =
            await identityRepository.GeneratePasswordResetTokenAsync(
                user,
                cancellationToken);

        var encodedToken = Convert.ToBase64String(
          Encoding.UTF8.GetBytes(token))
      .TrimEnd('=')
      .Replace('+', '-')
      .Replace('/', '_');

        var resetPasswordUrl =
            $"{frontendOrigin.TrimEnd('/')}/auth/reset-password" +
            $"?userId={user.Id}" +
            $"&token={Uri.EscapeDataString(encodedToken)}";

        var htmlBody = CreateEmailBody(
            WebUtility.HtmlEncode(user.FullName),
            resetPasswordUrl);

        await emailService.SendAsync(
            user.Email!,
            "Reset your SmartERP password",
            htmlBody,
            cancellationToken);

        return CreateResponse();
    }

    private static ForgotPasswordResponseDto CreateResponse()
    {
        return new ForgotPasswordResponseDto(ResponseMessage);
    }

    private static string CreateEmailBody(
        string fullName,
        string resetPasswordUrl)
    {
        return $"""
                <h2>Reset your password</h2>
                <p>Hello {fullName},</p>
                <p>Click the link below to reset your password:</p>
                <p>
                    <a href="{WebUtility.HtmlEncode(resetPasswordUrl)}">
                        Reset Password
                    </a>
                </p>
                <p>This link expires in one hour.</p>
                <p>If you did not request this, ignore this email.</p>
                """;
    }
}
