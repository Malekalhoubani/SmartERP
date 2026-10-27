using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.ResetPassword.DTO;
using SmartERP.Identity.Services.Features.Authentication.Sessions;
using SmartERP.Identity.Services.Interfaces;
using SmartERP.Identity.Services.Interfaces.Authentication;

namespace SmartERP.Identity.Services.Features.Authentication.ResetPassword.Implementations;

public sealed class ResetPasswordService(
    IIdentityRepository identityRepository,
    IAuthenticationSessionService sessionService,
    IEmailService emailService,
    ILogger<ResetPasswordService> logger)
    : IResetPasswordService
{
    public async Task<ResetPasswordResponseDto> ResetPasswordAsync(
        ResetPasswordRequestDto requestDto,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var user = await identityRepository.FindByIdAsync(
            requestDto.UserId,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new ArgumentException(
                "The password reset link is invalid or expired.");
        }

        var token = DecodeToken(requestDto.Token);

        var resetResult = await identityRepository.ResetPasswordAsync(
            user,
            token,
            requestDto.NewPassword,
            cancellationToken);

        if (!resetResult.Succeeded)
        {
            throw new ArgumentException(
                "The password reset link is invalid or expired.");
        }

        await sessionService.RevokeAllSessionsAsync(
            user.Id,
            "Password was reset.",
            ipAddress,
            cancellationToken);

        await SendSecurityNotificationAsync(user.Email!, user.FullName);

        return new ResetPasswordResponseDto(
            "Password reset successfully. Please sign in again.");
    }

    private async Task SendSecurityNotificationAsync(
        string email,
        string fullName)
    {
        try
        {
            var safeName = WebUtility.HtmlEncode(fullName);

            var htmlBody = $$"""
                <h2>Your SmartERP password was changed</h2>
                <p>Hello {{safeName}},</p>
                <p>Your password was reset successfully.</p>
                <p>All existing sessions were signed out for your security.</p>
                <p>If you did not make this change, contact support immediately.</p>
                """;

            await emailService.SendAsync(
                email,
                "Your SmartERP password was changed",
                htmlBody,
                CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Password reset notification could not be sent for user {Email}.",
                email);
        }
    }

    private static string DecodeToken(string encodedToken)
    {
        try
        {
            var base64 = encodedToken
                .Replace('-', '+')
                .Replace('_', '/');

            base64 = (base64.Length % 4) switch
            {
                2 => base64 + "==",
                3 => base64 + "=",
                _ => base64
            };

            return Encoding.UTF8.GetString(
                Convert.FromBase64String(base64));
        }
        catch (FormatException exception)
        {
            throw new ArgumentException(
                "The password reset link is invalid or expired.",
                exception);
        }
    }
}
