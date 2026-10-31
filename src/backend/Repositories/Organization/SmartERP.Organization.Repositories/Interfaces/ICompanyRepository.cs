using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<Company?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Company>> GetForUserAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<bool> IsCodeInUseAsync(
        string code,
        int? excludedCompanyId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Company company,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
