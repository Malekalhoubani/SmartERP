using FluentValidation;
using SmartERP.Identity.Services.Features.Authentication.Register.DTO;

namespace SmartERP.Identity.Services.Features.Authentication.Register;

public sealed class ResendConfirmationEmailRequestDtoValidator
    : AbstractValidator<ResendConfirmationEmailRequestDto>
{
    public ResendConfirmationEmailRequestDtoValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");
    }
}
