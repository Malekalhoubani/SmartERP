using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Repositories.Interfaces
{
    public  interface IRolePermissionRepository
    {
        Task<IReadOnlyCollection<ApplicationRole>> GetRolesAsync(CancellationToken cancellationToken = default);
        Task<ApplicationRole?> FindRoleByIdAsync(int roleId, CancellationToken cancellationToken = default);
        Task<IdentityOperationResult> CreateRoleAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<IdentityOperationResult> UpdateRoleAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<IdentityOperationResult> DeleteRoleAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default);
        Task SetRolePermissionsAsync(int roleId, IReadOnlyCollection<int> permissionIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<int>> GetUserRoleIdsAsync(int userId, CancellationToken cancellationToken = default);

        Task<IdentityOperationResult> SetUserRolesAsync(int userId,IReadOnlyCollection<int> roleIds,CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(int userId,CancellationToken cancellationToken = default);


    }
}
