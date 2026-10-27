using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Services.Authorization
{
    public static class PermissionNames
    {
        public const string UsersView = "identity.users.view";
        public const string UsersManageRoles ="identity.users.roles.manage";
        public const string RolesView ="identity.roles.view";
        public const string RolesCreate ="identity.roles.create";
        public const string RolesUpdate ="identity.roles.update";
        public const string RolesDelete ="identity.roles.delete";
        public const string RolesManagePermissions ="identity.roles.permissions.manage";
        public const string PermissionsView ="identity.permissions.view";

        public static IReadOnlyCollection<string> All { get; } =
[
    UsersView,
        UsersManageRoles,
        RolesView,
        RolesCreate,
        RolesUpdate,
        RolesDelete,
        RolesManagePermissions,
        PermissionsView
];
    
}
}
