using FluentValidation;
using SmartERP.Identity.Services.Features.Authentication
    .ForgotPassword.DTO;

namespace SmartERP.Identity.Services.Features.Authentication
    .ForgotPassword.Validators;

public sealed class ForgotPasswordRequestDtoValidator: AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestDtoValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters.");
    }
}