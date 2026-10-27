using FluentValidation;
using SmartERP.Identity.Services.Features.Authentication.ResetPassword.DTO;

namespace SmartERP.Identity.Services.Features.Authentication.ResetPassword;

public sealed class ResetPasswordRequestDtoValidator
    : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestDtoValidator()
    {
        RuleFor(request => request.UserId)
            .GreaterThan(0)
            .WithMessage("User id is invalid.");

        RuleFor(request => request.Token)
            .NotEmpty()
            .WithMessage("Reset token is required.");

        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(12)
            .WithMessage("Password must contain at least 12 characters.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain a number.")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain a special character.");

        RuleFor(request => request.ConfirmPassword)
            .Equal(request => request.NewPassword)
            .WithMessage("Password confirmation does not match.");
    }
}
