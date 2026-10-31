using Microsoft.EntityFrameworkCore;
using SmartERP.Organization.DataAccess.Contexts;
using SmartERP.Organization.Domain.Entities;
using SmartERP.Organization.Repositories.Interfaces;

namespace SmartERP.Organization.DataAccess.Repositories;

public sealed class CompanyRepository : ICompanyRepository
{
    private readonly OrganizationDbContext _dbContext;

    public CompanyRepository(OrganizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Company?> GetByIdAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Companies.SingleOrDefaultAsync(
            company => company.Id == companyId,
            cancellationToken);
    }

    public Task<Company?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();

        return _dbContext.Companies
            .AsNoTracking()
            .SingleOrDefaultAsync(
                company => company.Code == normalizedCode,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Company>> GetForUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Companies
            .AsNoTracking()
            .Where(company => company.Memberships.Any(membership =>
                membership.UserId == userId && membership.IsActive))
            .OrderBy(company => company.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsCodeInUseAsync(
        string code,
        int? excludedCompanyId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = _dbContext.Companies
            .Where(company => company.Code == normalizedCode);

        if (excludedCompanyId.HasValue)
        {
            query = query.Where(company =>
                company.Id != excludedCompanyId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Company company,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Companies.AddAsync(
            company,
            cancellationToken);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
