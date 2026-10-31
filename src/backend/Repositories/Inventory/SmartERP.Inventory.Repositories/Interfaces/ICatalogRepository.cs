using SmartERP.Inventory.Domain.Entities;
using SmartERP.Inventory.Domain.Enums;

namespace SmartERP.Inventory.Repositories.Interfaces;

public interface ICatalogRepository
{
    Task<IReadOnlyList<CatalogItem>> ListAsync(int companyId,CatalogItemType type,CancellationToken cancellationToken = default);
    Task<CatalogItem?> FindAsync(int companyId,int itemId,CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(int companyId,CatalogItemType type,string code,int? excludedId = null,CancellationToken cancellationToken = default);
    Task AddAsync(CatalogItem item,CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
