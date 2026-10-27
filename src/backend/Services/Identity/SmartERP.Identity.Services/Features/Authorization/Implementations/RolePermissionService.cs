using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Repositories.Models;
using SmartERP.Identity.Services.Features.Authorization.DTO;
using SmartERP.Identity.Services.Interfaces.Authorization;

namespace SmartERP.Identity.Services.Features.Authorization.Implementations;

public sealed class RolePermissionService
    : IRolePermissionService
{
    private readonly IRolePermissionRepository _repository;
    private readonly IIdentityRepository _identityRepository;

    public RolePermissionService(
        IRolePermissionRepository repository,
        IIdentityRepository identityRepository)
    {
        _repository = repository;
        _identityRepository = identityRepository;
    }

    public async Task<IReadOnlyCollection<RoleResponseDto>>
        GetRolesAsync(
            CancellationToken cancellationToken = default)
    {
        var roles = await _repository.GetRolesAsync(
            cancellationToken);

        return roles
            .Select(MapRole)
            .ToArray();
    }

    public async Task<RoleResponseDto> GetRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default)
    {
        var role = await GetRequiredRoleAsync(
            roleId,
            cancellationToken);

        return MapRole(role);
    }

    public async Task<RoleResponseDto> CreateRoleAsync(
        CreateRoleRequestDto requestDto,
        CancellationToken cancellationToken = default)
    {
        var role = new ApplicationRole
        {
            Name = requestDto.Name.Trim(),

            Description = NormalizeDescription(
                requestDto.Description),

            IsActive = true,
            IsSystem = false,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var result = await _repository.CreateRoleAsync(
            role,
            cancellationToken);

        EnsureSucceeded(result);

        return MapRole(role);
    }

    public async Task<RoleResponseDto> UpdateRoleAsync(
        int roleId,
        UpdateRoleRequestDto requestDto,
        CancellationToken cancellationToken = default)
    {
        var role = await GetRequiredRoleAsync(
            roleId,
            cancellationToken);

        var roleName = requestDto.Name.Trim();

        if (role.IsSystem &&
            (!string.Equals(
                 role.Name,
                 roleName,
                 StringComparison.OrdinalIgnoreCase) ||
             !requestDto.IsActive))
        {
            throw new InvalidOperationException(
                "System roles cannot be renamed or deactivated.");
        }

        role.Name = roleName;

        role.Description = NormalizeDescription(
            requestDto.Description);

        role.IsActive = requestDto.IsActive;

        var result = await _repository.UpdateRoleAsync(
            role,
            cancellationToken);

        EnsureSucceeded(result);

        return MapRole(role);
    }

    public async Task DeleteRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default)
    {
        var role = await GetRequiredRoleAsync(
            roleId,
            cancellationToken);

        if (role.IsSystem)
        {
            throw new InvalidOperationException(
                "System roles cannot be deleted.");
        }

        var result = await _repository.DeleteRoleAsync(
            role,
            cancellationToken);

        EnsureSucceeded(result);
    }

    public async Task<IReadOnlyCollection<PermissionResponseDto>>
        GetPermissionsAsync(
            CancellationToken cancellationToken = default)
    {
        var permissions =
            await _repository.GetPermissionsAsync(
                cancellationToken);

        return permissions
            .Select(permission =>
                new PermissionResponseDto(
                    permission.Id,
                    permission.Name,
                    permission.DisplayName,
                    permission.Module,
                    permission.Description))
            .ToArray();
    }

    public async Task<RoleResponseDto>
        AssignPermissionsToRoleAsync(
            int roleId,
            AssignPermissionsToRoleRequestDto requestDto,
            CancellationToken cancellationToken = default)
    {
        await GetRequiredRoleAsync(
            roleId,
            cancellationToken);

        await _repository.SetRolePermissionsAsync(
            roleId,
            requestDto.PermissionIds,
            cancellationToken);

        var updatedRole = await GetRequiredRoleAsync(
            roleId,
            cancellationToken);

        return MapRole(updatedRole);
    }

    public async Task<UserRolesResponseDto> GetUserRolesAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _identityRepository.FindByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User was not found.");
        }

        var roleIds = await _repository.GetUserRoleIdsAsync(
            userId,
            cancellationToken);

        return new UserRolesResponseDto(
            userId,
            roleIds);
    }

    public async Task<UserRolesResponseDto>
        AssignRolesToUserAsync(
            int userId,
            AssignRolesToUserRequestDto requestDto,
            CancellationToken cancellationToken = default)
    {
        var result = await _repository.SetUserRolesAsync(
            userId,
            requestDto.RoleIds,
            cancellationToken);

        EnsureSucceeded(result);

        var roleIds = await _repository.GetUserRoleIdsAsync(
            userId,
            cancellationToken);

        return new UserRolesResponseDto(
            userId,
            roleIds);
    }

    private async Task<ApplicationRole> GetRequiredRoleAsync(
        int roleId,
        CancellationToken cancellationToken)
    {
        var role = await _repository.FindRoleByIdAsync(
            roleId,
            cancellationToken);

        return role
            ?? throw new KeyNotFoundException(
                "Role was not found.");
    }

    private static RoleResponseDto MapRole(
        ApplicationRole role)
    {
        var permissions = role.RolePermissions
            .Where(rolePermission =>
                rolePermission.Permission.IsActive)
            .Select(rolePermission =>
                rolePermission.Permission.Name)
            .OrderBy(permissionName => permissionName)
            .ToArray();

        return new RoleResponseDto(
            role.Id,
            role.Name!,
            role.Description,
            role.IsActive,
            role.IsSystem,
            permissions);
    }

    private static string? NormalizeDescription(
        string? description)
    {
        return string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }

    private static void EnsureSucceeded(
        IdentityOperationResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new InvalidOperationException(
            string.Join(" ", result.Errors));
    }
}