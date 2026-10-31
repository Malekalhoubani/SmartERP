using Microsoft.EntityFrameworkCore;
using SmartERP.Inventory.DataAccess.Contexts;
using SmartERP.Inventory.Domain.Entities;
using SmartERP.Inventory.Domain.Enums;
using SmartERP.Inventory.Repositories.Interfaces;

namespace SmartERP.Inventory.DataAccess.Repositories;

public sealed class CatalogRepository(InventoryDbContext dbContext) : ICatalogRepository
{
    public async Task<IReadOnlyList<CatalogItem>> ListAsync(int companyId,CatalogItemType type,CancellationToken cancellationToken = default) =>
        await dbContext.CatalogItems.AsNoTracking()
            .Where(item => item.CompanyId == companyId && item.Type == type)
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

    public Task<CatalogItem?> FindAsync(int companyId,int itemId,CancellationToken cancellationToken = default) =>
        dbContext.CatalogItems.SingleOrDefaultAsync(item => item.CompanyId == companyId && item.Id == itemId,cancellationToken);

    public Task<bool> CodeExistsAsync(int companyId,CatalogItemType type,string code,int? excludedId = null,CancellationToken cancellationToken = default) =>
        dbContext.CatalogItems.AnyAsync(item => item.CompanyId == companyId && item.Type == type && item.Code == code && (!excludedId.HasValue || item.Id != excludedId.Value),cancellationToken);

    public Task AddAsync(CatalogItem item,CancellationToken cancellationToken = default) => dbContext.CatalogItems.AddAsync(item,cancellationToken).AsTask();
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
