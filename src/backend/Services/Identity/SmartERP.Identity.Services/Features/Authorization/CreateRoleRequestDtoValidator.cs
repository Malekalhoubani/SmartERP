using FluentValidation;

namespace SmartERP.Identity.Services.Features.Authorization.DTO;

public sealed class CreateRoleRequestDtoValidator: AbstractValidator<CreateRoleRequestDto>
{
    public CreateRoleRequestDtoValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[\p{L}][\p{L}\p{N}._ -]*$")
            .WithMessage(
                "Role name contains invalid characters.");

        RuleFor(request => request.Description)
            .MaximumLength(250);
    }
}