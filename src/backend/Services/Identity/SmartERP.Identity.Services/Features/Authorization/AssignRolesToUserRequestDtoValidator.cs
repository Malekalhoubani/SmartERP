using FluentValidation;

namespace SmartERP.Identity.Services.Features.Authorization.DTO;

public sealed class AssignRolesToUserRequestDtoValidator: AbstractValidator<AssignRolesToUserRequestDto>
{
    public AssignRolesToUserRequestDtoValidator()
    {
        RuleFor(request => request.RoleIds)
            .NotNull();

        RuleForEach(request => request.RoleIds)
            .GreaterThan(0);
    }
}