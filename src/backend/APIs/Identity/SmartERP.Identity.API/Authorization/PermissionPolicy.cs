namespace SmartERP.Identity.API.Authorization;

public static class PermissionPolicy
{
    public const string Prefix = "Permission:";

    public static string Create(string permission)
    {
        return $"{Prefix}{permission}";
    }
}
