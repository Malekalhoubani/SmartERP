using System.Text.Encodings.Web;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.Register.DTO;
using SmartERP.Identity.Services.Interfaces;

namespace SmartERP.Identity.Services.Features.Authentication.Register;

public sealed class RegisterService : IRegisterService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IEmailService _emailService;

    public RegisterService(
        IIdentityRepository identityRepository,
        IEmailService emailService)
    {
        _identityRepository = identityRepository;
        _emailService = emailService;
    }

    public async Task<RegisterResponseDto> RegisterAsync(
        RegisterRequestDto requestDto,
        string confirmationBaseUrl,
        CancellationToken cancellationToken = default)
    {
        var email = requestDto.Email.Trim();
        var existingUser = await _identityRepository.FindByEmailAsync(
            email,
            cancellationToken);

        if (existingUser is not null)
        {
            if (existingUser.EmailConfirmed)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            await SendConfirmationEmailAsync(
                existingUser,
                confirmationBaseUrl,
                cancellationToken);

            return CreateResponse(existingUser);
        }

        var user = new ApplicationUser
        {
            FullName = requestDto.FullName.Trim(),
            Email = email,
            UserName = email,
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var creationResult = await _identityRepository.CreateUserAsync(
            user,
            requestDto.Password,
            cancellationToken);

        if (!creationResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(" ", creationResult.Errors));
        }

        await SendConfirmationEmailAsync(
            user,
            confirmationBaseUrl,
            cancellationToken);

        return CreateResponse(user);
    }

    public async Task ConfirmEmailAsync(
        int userId,
        string token,
        CancellationToken cancellationToken = default)
    {
        if (userId <= 0 || string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException(
                "The email confirmation link is invalid.");
        }

        var user = await _identityRepository.FindByIdAsync(
            userId,
            cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        if (user.EmailConfirmed)
        {
            return;
        }

        var confirmationResult = await _identityRepository.ConfirmEmailAsync(
            user,
            token,
            cancellationToken);

        if (!confirmationResult.Succeeded)
        {
            throw new ArgumentException(
                "The email confirmation token is invalid or expired.");
        }
    }

    private async Task SendConfirmationEmailAsync(
        ApplicationUser user,
        string confirmationBaseUrl,
        CancellationToken cancellationToken)
    {
        var token = await _identityRepository
            .GenerateEmailConfirmationTokenAsync(user, cancellationToken);

        var confirmationLink =
            $"{confirmationBaseUrl}?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        var safeName = HtmlEncoder.Default.Encode(user.FullName);
        var safeLink = HtmlEncoder.Default.Encode(confirmationLink);

        var htmlBody = $$"""
            <!doctype html>
            <html lang="en">
            <body style="font-family:Arial,sans-serif;color:#1f2937">
                <h2>Welcome to SmartERP, {{safeName}}</h2>
                <p>Please confirm your email address to activate your account.</p>
                <p>
                    <a href="{{safeLink}}"
                       style="display:inline-block;padding:12px 20px;background:#2563eb;color:#ffffff;text-decoration:none;border-radius:6px">
                        Confirm email
                    </a>
                </p>
                <p>This link expires after one hour.</p>
                <p>If you did not create this account, ignore this message.</p>
            </body>
            </html>
            """;

        await _emailService.SendAsync(
            user.Email!,
            "Confirm your SmartERP account",
            htmlBody,
            cancellationToken);
    }

    private static RegisterResponseDto CreateResponse(ApplicationUser user)
    {
        return new RegisterResponseDto(
            user.Id,
            user.Email!,
            RequiresEmailConfirmation: true);
    }
}
