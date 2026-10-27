using FluentValidation;

namespace SmartERP.Identity.Services.Features.Authentication.Login.DTO;

public sealed class LoginRequestDtoValidator: AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
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
            .MaximumLength(128)
            .WithMessage("Password must not exceed 128 characters.");
    }
}