using FluentValidation;

using SmartERP.Identity.Services.Features.Authentication.Register.DTO;

namespace SmartERP.Identity.Services.Features.Authentication.Register;

public sealed class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(request => request.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MinimumLength(2)
            .WithMessage("Full name must contain at least 2 characters.")
            .MaximumLength(150)
            .WithMessage("Full name must not exceed 150 characters.")
            .Matches(@"^[\p{L}\p{M} .'-]+$")
            .WithMessage("Full name contains invalid characters.");

        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(12)
            .WithMessage("Password must contain at least 12 characters.")
            .MaximumLength(128)
            .WithMessage("Password must not exceed 128 characters.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain a number.")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain a special character.")
            .Must(password => !password.Any(char.IsWhiteSpace))
            .WithMessage("Password must not contain spaces.");

        RuleFor(request => request.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Password confirmation is required.")
            .Equal(request => request.Password)
            .WithMessage("Password and confirmation do not match.");
    }
}
