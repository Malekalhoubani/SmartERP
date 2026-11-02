using Microsoft.EntityFrameworkCore;
using SmartERP.Inventory.DataAccess.Contexts;
using SmartERP.Inventory.Domain.Entities;
using SmartERP.Inventory.Domain.Enums;
using SmartERP.Inventory.Repositories.Interfaces;

namespace SmartERP.Inventory.DataAccess.Repositories;

public sealed class OperationalRecordRepository(InventoryDbContext dbContext) : IOperationalRecordRepository
{
    public async Task<IReadOnlyList<OperationalRecord>> ListAsync(int companyId, OperationalRecordType type, CancellationToken cancellationToken = default) =>
        await dbContext.OperationalRecords.AsNoTracking()
            .Where(record => record.CompanyId == companyId && record.Type == type)
            .OrderByDescending(record => record.DocumentDateUtc ?? record.CreatedAtUtc)
            .ThenBy(record => record.Name)
            .ToListAsync(cancellationToken);

    public Task<OperationalRecord?> FindAsync(int companyId, long recordId, CancellationToken cancellationToken = default) =>
        dbContext.OperationalRecords.SingleOrDefaultAsync(record => record.CompanyId == companyId && record.Id == recordId, cancellationToken);

    public Task<bool> CodeExistsAsync(int companyId, OperationalRecordType type, string code, long? excludedId = null, CancellationToken cancellationToken = default) =>
        dbContext.OperationalRecords.AnyAsync(record => record.CompanyId == companyId && record.Type == type && record.Code == code && (!excludedId.HasValue || record.Id != excludedId.Value), cancellationToken);

    public Task AddAsync(OperationalRecord record, CancellationToken cancellationToken = default) => dbContext.OperationalRecords.AddAsync(record, cancellationToken).AsTask();
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
