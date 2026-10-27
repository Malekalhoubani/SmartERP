using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Repositories.Interfaces
{
    public interface IIdentityRepository
    {
        Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<ApplicationUser?> FindByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IdentityOperationResult> CreateUserAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
        Task<IdentityOperationResult> ConfirmEmailAsync(ApplicationUser user, string token, CancellationToken cancellationToken = default);
        Task<PasswordCheckResult> CheckPasswordAsync(ApplicationUser user, string password,CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<string>> GetRolesAsync(ApplicationUser user , CancellationToken cancellationToken = default);
    }
}
