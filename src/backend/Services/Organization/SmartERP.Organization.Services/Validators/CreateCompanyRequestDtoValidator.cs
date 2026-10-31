using FluentValidation;
using SmartERP.Organization.Services.DTO.Company;

namespace SmartERP.Organization.Services.Validators.Company;

public sealed class CreateCompanyRequestDtoValidator: AbstractValidator<CreateCompanyRequestDto>
{
    public CreateCompanyRequestDtoValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.LegalName)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(request => request.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage(
                "Code may contain letters, numbers, hyphens and underscores only.");

        RuleFor(request => request.RegistrationNumber)
            .MaximumLength(100);

        RuleFor(request => request.TaxNumber)
            .MaximumLength(100);

        RuleFor(request => request.Email)
            .MaximumLength(256)
            .EmailAddress()
            .When(request =>
                !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.Phone)
            .MaximumLength(30);
    }
}
