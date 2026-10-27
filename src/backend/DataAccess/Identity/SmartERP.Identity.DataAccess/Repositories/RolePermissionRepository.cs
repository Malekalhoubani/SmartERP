using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartERP.Identity.DataAccess.Contexts;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.DataAccess.Repositories
{
    public sealed class RolePermissionRepository  : IRolePermissionRepository
    {
        private readonly IdentityApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolePermissionRepository(IdentityApplicationDbContext dbContext,UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<IReadOnlyCollection<ApplicationRole>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles.AsNoTracking().Include(role => role.RolePermissions)
             .ThenInclude(rolePermission =>
             rolePermission.Permission)
            .OrderBy(role => role.Name)
            .ToArrayAsync(cancellationToken);
        }

        public Task<ApplicationRole?> FindRoleByIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Roles
                .Include(role => role.RolePermissions)
                    .ThenInclude(rolePermission => rolePermission.Permission)
                .SingleOrDefaultAsync(
                    role => role.Id == roleId,
                    cancellationToken);
        }

        public async Task<IdentityOperationResult> CreateRoleAsync(ApplicationRole role , CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _roleManager.CreateAsync(role);

            return ConvertResult(result);
        }

        public async Task<IdentityOperationResult> UpdateRoleAsync(ApplicationRole role,CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _roleManager.UpdateAsync(role);

            return ConvertResult(result);
        }

        public async Task<IdentityOperationResult> DeleteRoleAsync(ApplicationRole role,CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _roleManager.DeleteAsync(role);

            return ConvertResult(result);
        }

        public async Task<IReadOnlyCollection<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Permissions.AsNoTracking().Where(permission=>permission.IsActive).OrderBy(permission=>permission.Module).ThenBy(permission=>permission.Name).ToArrayAsync(cancellationToken);
        }

        public async Task SetRolePermissionsAsync(int roleId, IReadOnlyCollection<int> permissionIds, CancellationToken cancellationToken= default)
        {
            var requestedIds = permissionIds.Distinct().ToArray();

            var existingPermissionIds = await _dbContext.Permissions
                .AsNoTracking()
                .Where(permission =>
                    requestedIds.Contains(permission.Id) &&
                    permission.IsActive)
                .Select(permission => permission.Id)
                .ToArrayAsync(cancellationToken);


            if (existingPermissionIds.Length != requestedIds.Length)
            {
                throw new KeyNotFoundException(
                    "One or more permissions were not found.");
            }

            var currentMappings =await _dbContext.RolePermissions.Where(rolePermission =>
            rolePermission.RoleId == roleId)
           .ToListAsync(cancellationToken);

            _dbContext.RolePermissions.RemoveRange(currentMappings);

            var newMappings = requestedIds
         .Select(permissionId => new RolePermission
         {
             RoleId = roleId,
             PermissionId = permissionId
         });

            await _dbContext.RolePermissions.AddRangeAsync(
                newMappings,
                cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

        }

        public async Task<IReadOnlyCollection<int>>GetUserRoleIdsAsync(int userId,CancellationToken cancellationToken = default)
        {
            return await _dbContext
                .Set<IdentityUserRole<int>>()
                .Where(userRole =>
                    userRole.UserId == userId)
                .Select(userRole => userRole.RoleId)
                .ToArrayAsync(cancellationToken);
        }


        public async Task<IdentityOperationResult> SetUserRolesAsync(int userId, IReadOnlyCollection<int> roleIds,CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(
                userId.ToString());

            if (user is null)
            {
                throw new KeyNotFoundException(
                    "User was not found.");
            }

            var requestedIds = roleIds
                .Distinct()
                .ToArray();

            var roles = await _dbContext.Roles
                .Where(role =>
                    requestedIds.Contains(role.Id) &&
                    role.IsActive)
                .ToArrayAsync(cancellationToken);

            if (roles.Length != requestedIds.Length)
            {
                throw new KeyNotFoundException(
                    "One or more roles were not found.");
            }

            var currentRoleNames =
                await _userManager.GetRolesAsync(user);

            var requestedRoleNames = roles
                .Select(role => role.Name!)
                .ToArray();

            var rolesToRemove = currentRoleNames
                .Except(requestedRoleNames)
                .ToArray();

            var rolesToAdd = requestedRoleNames
                .Except(currentRoleNames)
                .ToArray();

            var removeResult =
                await _userManager.RemoveFromRolesAsync(
                    user,
                    rolesToRemove);

            if (!removeResult.Succeeded)
            {
                return ConvertResult(removeResult);
            }

            var addResult =
                await _userManager.AddToRolesAsync(
                    user,
                    rolesToAdd);

            return ConvertResult(addResult);
        }

        public async Task<IReadOnlyCollection<string>>GetUserPermissionsAsync(int userId,CancellationToken cancellationToken = default)
        {
            var roleIds = _dbContext
                .Set<IdentityUserRole<int>>()
                .Where(userRole =>
                    userRole.UserId == userId)
                .Select(userRole => userRole.RoleId);

            return await _dbContext.RolePermissions
                .AsNoTracking()
                .Where(rolePermission =>
                    roleIds.Contains(rolePermission.RoleId) &&
                    rolePermission.Role.IsActive &&
                    rolePermission.Permission.IsActive)
                .Select(rolePermission =>
                    rolePermission.Permission.Name)
                .Distinct()
                .OrderBy(permissionName => permissionName)
                .ToArrayAsync(cancellationToken);
        }

        private static IdentityOperationResult ConvertResult(IdentityResult result)
        {
            return result.Succeeded
                ? IdentityOperationResult.Success()
                : IdentityOperationResult.Failure(
                    result.Errors.Select(error =>
                        error.Description));
        }
    }
}
