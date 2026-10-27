using SmartERP.Identity.API.Authorization;
using SmartERP.Identity.API.Filters;
using SmartERP.Identity.Services.Authorization;
using SmartERP.Identity.Services.Features.Authorization.DTO;
using SmartERP.Identity.Services.Interfaces.Authorization;

namespace SmartERP.Identity.API.Endpoints;

public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var adminGroup = endpoints
            .MapGroup("/api/admin")
            .WithTags("Roles and Permissions")
            .RequireAuthorization();

        adminGroup
            .MapGet("/roles", GetRolesAsync)
            .RequirePermission(PermissionNames.RolesView);

        adminGroup
            .MapGet("/roles/{roleId:int}", GetRoleAsync)
            .RequirePermission(PermissionNames.RolesView);

        adminGroup
            .MapPost("/roles", CreateRoleAsync)
            .AddEndpointFilter<ValidationFilter<CreateRoleRequestDto>>()
            .RequirePermission(PermissionNames.RolesCreate);

        adminGroup
            .MapPut("/roles/{roleId:int}", UpdateRoleAsync)
            .AddEndpointFilter<ValidationFilter<UpdateRoleRequestDto>>()
            .RequirePermission(PermissionNames.RolesUpdate);

        adminGroup
            .MapDelete("/roles/{roleId:int}", DeleteRoleAsync)
            .RequirePermission(PermissionNames.RolesDelete);

        adminGroup
            .MapGet("/permissions", GetPermissionsAsync)
            .RequirePermission(PermissionNames.PermissionsView);

        adminGroup
            .MapPut(
                "/roles/{roleId:int}/permissions",
                AssignPermissionsToRoleAsync)
            .AddEndpointFilter<
                ValidationFilter<AssignPermissionsToRoleRequestDto>>()
            .RequirePermission(
                PermissionNames.RolesManagePermissions);

        adminGroup
            .MapGet("/users/{userId:int}/roles", GetUserRolesAsync)
            .RequirePermission(PermissionNames.UsersView);

        adminGroup
            .MapPut("/users/{userId:int}/roles", AssignRolesToUserAsync)
            .AddEndpointFilter<
                ValidationFilter<AssignRolesToUserRequestDto>>()
            .RequirePermission(PermissionNames.UsersManageRoles);

        return endpoints;
    }

    private static async Task<IResult> GetRolesAsync(
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(
            await service.GetRolesAsync(cancellationToken));
    }

    private static async Task<IResult> GetRoleAsync(
        int roleId,
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(
            await service.GetRoleAsync(
                roleId,
                cancellationToken));
    }

    private static async Task<IResult> CreateRoleAsync(
        CreateRoleRequestDto requestDto,
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        var response = await service.CreateRoleAsync(
            requestDto,
            cancellationToken);

        return Results.Created(
            $"/api/admin/roles/{response.Id}",
            response);
    }

    private static async Task<IResult> UpdateRoleAsync(
        int roleId,
        UpdateRoleRequestDto requestDto,
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(
            await service.UpdateRoleAsync(
                roleId,
                requestDto,
                cancellationToken));
    }

    private static async Task<IResult> DeleteRoleAsync(
        int roleId,
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        await service.DeleteRoleAsync(
            roleId,
            cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> GetPermissionsAsync(
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(
            await service.GetPermissionsAsync(cancellationToken));
    }

    private static async Task<IResult> AssignPermissionsToRoleAsync(
        int roleId,
        AssignPermissionsToRoleRequestDto requestDto,
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(
            await service.AssignPermissionsToRoleAsync(
                roleId,
                requestDto,
                cancellationToken));
    }

    private static async Task<IResult> GetUserRolesAsync(
        int userId,
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(
            await service.GetUserRolesAsync(
                userId,
                cancellationToken));
    }

    private static async Task<IResult> AssignRolesToUserAsync(
        int userId,
        AssignRolesToUserRequestDto requestDto,
        IRolePermissionService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(
            await service.AssignRolesToUserAsync(
                userId,
                requestDto,
                cancellationToken));
    }
}
