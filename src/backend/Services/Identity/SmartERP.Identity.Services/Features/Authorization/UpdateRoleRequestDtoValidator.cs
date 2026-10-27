using FluentValidation;
using SmartERP.Identity.Services.Features.Authorization.DTO;

namespace SmartERP.Identity.Services.Features.Authorization;

public sealed class UpdateRoleRequestDtoValidator: AbstractValidator<UpdateRoleRequestDto>
{
    public UpdateRoleRequestDtoValidator()
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