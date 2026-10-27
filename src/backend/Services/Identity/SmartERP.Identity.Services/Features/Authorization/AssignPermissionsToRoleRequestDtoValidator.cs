using FluentValidation;

namespace SmartERP.Identity.Services.Features.Authorization.DTO;

public sealed class AssignPermissionsToRoleRequestDtoValidator: AbstractValidator<AssignPermissionsToRoleRequestDto>
{
    public AssignPermissionsToRoleRequestDtoValidator()
    {
        RuleFor(request => request.PermissionIds)
            .NotNull();

        RuleForEach(request => request.PermissionIds)
            .GreaterThan(0);
    }
}