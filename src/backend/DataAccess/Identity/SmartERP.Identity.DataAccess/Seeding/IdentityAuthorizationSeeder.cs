using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartERP.Identity.DataAccess.Contexts;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Services.Authorization;

namespace SmartERP.Identity.DataAccess.Seeding;

public sealed class IdentityAuthorizationSeeder(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<IdentityAuthorizationSeeder> logger)
    : IHostedService
{
    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<IdentityApplicationDbContext>();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<ApplicationRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        await SeedRolesAsync(roleManager, cancellationToken);
        await SeedPermissionsAsync(dbContext, cancellationToken);
        await SeedRolePermissionsAsync(dbContext, cancellationToken);
        await AssignConfiguredSuperAdminAsync(
            userManager,
            cancellationToken);

        logger.LogInformation(
            "Identity roles and permissions were seeded successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task SeedRolesAsync(
        RoleManager<ApplicationRole> roleManager,
        CancellationToken cancellationToken)
    {
        foreach (var roleName in SystemRoleNames.All)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var role = await roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                var createResult = await roleManager.CreateAsync(
                    new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"System role: {roleName}",
                        IsActive = true,
                        IsSystem = true,
                        CreatedAtUtc = DateTimeOffset.UtcNow
                    });

                EnsureSucceeded(createResult);
                continue;
            }

            if (!role.IsSystem || !role.IsActive)
            {
                role.IsSystem = true;
                role.IsActive = true;

                EnsureSucceeded(
                    await roleManager.UpdateAsync(role));
            }
        }
    }

    private static async Task SeedPermissionsAsync(
        IdentityApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var definitions = GetPermissionDefinitions();
        var existingPermissions = await dbContext.Permissions
            .ToDictionaryAsync(
                permission => permission.Name,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        foreach (var definition in definitions)
        {
            if (existingPermissions.TryGetValue(
                    definition.Name,
                    out var permission))
            {
                permission.DisplayName = definition.DisplayName;
                permission.Module = definition.Module;
                permission.IsActive = true;
                continue;
            }

            await dbContext.Permissions.AddAsync(
                new Permission
                {
                    Name = definition.Name,
                    DisplayName = definition.DisplayName,
                    Module = definition.Module,
                    IsActive = true,
                    CreatedAtUtc = DateTimeOffset.UtcNow
                },
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedRolePermissionsAsync(
        IdentityApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var roles = await dbContext.Roles
            .Where(role => role.IsSystem)
            .ToDictionaryAsync(
                role => role.Name!,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        var permissions = await dbContext.Permissions
            .Where(permission => permission.IsActive)
            .ToDictionaryAsync(
                permission => permission.Name,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        await AddPermissionsAsync(
            dbContext,
            roles[SystemRoleNames.SuperAdmin].Id,
            permissions.Values.Select(permission => permission.Id),
            cancellationToken);

        await AddPermissionsAsync(
            dbContext,
            roles[SystemRoleNames.Admin].Id,
            permissions.Values
                .Where(permission =>
                    permission.Name != PermissionNames.RolesDelete)
                .Select(permission => permission.Id),
            cancellationToken);

        var managerPermissionNames = new[]
        {
            PermissionNames.UsersView,
            PermissionNames.RolesView,
            PermissionNames.PermissionsView
        };

        await AddPermissionsAsync(
            dbContext,
            roles[SystemRoleNames.Manager].Id,
            managerPermissionNames.Select(name => permissions[name].Id),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AssignConfiguredSuperAdminAsync(
        UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken)
    {
        var email = configuration[
            "AuthorizationSeed:SuperAdminEmail"];

        if (string.IsNullOrWhiteSpace(email))
        {
            logger.LogWarning(
                "AuthorizationSeed:SuperAdminEmail is not configured. No user was assigned to SuperAdmin.");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(email.Trim());

        if (user is null)
        {
            logger.LogWarning(
                "Configured SuperAdmin user was not found.");
            return;
        }

        if (!await userManager.IsInRoleAsync(
                user,
                SystemRoleNames.SuperAdmin))
        {
            EnsureSucceeded(
                await userManager.AddToRoleAsync(
                    user,
                    SystemRoleNames.SuperAdmin));
        }
    }

    private static async Task AddPermissionsAsync(
        IdentityApplicationDbContext dbContext,
        int roleId,
        IEnumerable<int> permissionIds,
        CancellationToken cancellationToken)
    {
        var existingIds = await dbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == roleId)
            .Select(rolePermission => rolePermission.PermissionId)
            .ToArrayAsync(cancellationToken);

        var mappings = permissionIds
            .Except(existingIds)
            .Select(permissionId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });

        await dbContext.RolePermissions.AddRangeAsync(
            mappings,
            cancellationToken);
    }

    private static IReadOnlyCollection<PermissionDefinition>
        GetPermissionDefinitions()
    {
        return
        [
            new(PermissionNames.UsersView, "View users", "Identity"),
            new(
                PermissionNames.UsersManageRoles,
                "Manage user roles",
                "Identity"),
            new(PermissionNames.RolesView, "View roles", "Identity"),
            new(PermissionNames.RolesCreate, "Create roles", "Identity"),
            new(PermissionNames.RolesUpdate, "Update roles", "Identity"),
            new(PermissionNames.RolesDelete, "Delete roles", "Identity"),
            new(
                PermissionNames.RolesManagePermissions,
                "Manage role permissions",
                "Identity"),
            new(
                PermissionNames.PermissionsView,
                "View permissions",
                "Identity")
        ];
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new InvalidOperationException(
            string.Join(
                " ",
                result.Errors.Select(error => error.Description)));
    }

    private sealed record PermissionDefinition(
        string Name,
        string DisplayName,
        string Module);
}
