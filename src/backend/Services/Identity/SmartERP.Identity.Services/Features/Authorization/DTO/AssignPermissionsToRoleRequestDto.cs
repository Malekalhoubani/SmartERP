namespace SmartERP.Identity.Services.Features.Authorization.DTO;

public sealed record AssignPermissionsToRoleRequestDto(IReadOnlyCollection<int> PermissionIds);