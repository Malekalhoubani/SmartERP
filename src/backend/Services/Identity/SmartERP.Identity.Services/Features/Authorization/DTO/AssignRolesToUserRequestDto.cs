namespace SmartERP.Identity.Services.Features.Authorization.DTO;

public sealed record AssignRolesToUserRequestDto(IReadOnlyCollection<int> RoleIds);