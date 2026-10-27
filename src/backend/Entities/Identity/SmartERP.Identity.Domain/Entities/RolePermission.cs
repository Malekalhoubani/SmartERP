namespace SmartERP.Identity.Domain.Entities;

public sealed class RolePermission
{
    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public ApplicationRole Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}