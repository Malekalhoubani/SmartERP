namespace SmartERP.Identity.Services.Features.Authorization.DTO;

public sealed record UserRolesResponseDto(int UserId,IReadOnlyCollection<int> RoleIds);