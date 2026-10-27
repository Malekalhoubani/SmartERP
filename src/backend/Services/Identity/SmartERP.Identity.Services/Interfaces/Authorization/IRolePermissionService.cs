using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartERP.Identity.Services.Features.Authorization.DTO;

namespace SmartERP.Identity.Services.Interfaces.Authorization;

public interface IRolePermissionService
{
    Task<IReadOnlyCollection<RoleResponseDto>> GetRolesAsync(
        CancellationToken cancellationToken = default);

    Task<RoleResponseDto> GetRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default);

    Task<RoleResponseDto> CreateRoleAsync(
        CreateRoleRequestDto requestDto,
        CancellationToken cancellationToken = default);

    Task<RoleResponseDto> UpdateRoleAsync(
        int roleId,
        UpdateRoleRequestDto requestDto,
        CancellationToken cancellationToken = default);

    Task DeleteRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PermissionResponseDto>> GetPermissionsAsync(
        CancellationToken cancellationToken = default);

    Task<RoleResponseDto> AssignPermissionsToRoleAsync(
        int roleId,
        AssignPermissionsToRoleRequestDto requestDto,
        CancellationToken cancellationToken = default);

    Task<UserRolesResponseDto> GetUserRolesAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<UserRolesResponseDto> AssignRolesToUserAsync(
        int userId,
        AssignRolesToUserRequestDto requestDto,
        CancellationToken cancellationToken = default);
}
