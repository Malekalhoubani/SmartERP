namespace SmartERP.Identity.Services.Authorization;

public static class SystemRoleNames
{
    public const string SuperAdmin ="SuperAdmin";

    public const string Admin ="Admin";

    public const string Manager ="Manager";

    public const string User ="User";

    public static IReadOnlyCollection<string> All { get; } =
    [
        SuperAdmin,
        Admin,
        Manager,
        User
    ];
}