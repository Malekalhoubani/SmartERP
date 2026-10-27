namespace SmartERP.Identity.Services.Features.Authorization.DTO;

public sealed record UpdateRoleRequestDto(string Name,string? Description,bool IsActive);