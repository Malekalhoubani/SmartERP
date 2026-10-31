using SmartERP.Inventory.Domain.Enums;
using SmartERP.Inventory.Services.DTO;

namespace SmartERP.Inventory.Services.Interfaces;

public interface ICatalogService
{
    Task<IReadOnlyList<CatalogItemResponseDto>> ListAsync(int companyId,CatalogItemType type,CancellationToken cancellationToken = default);
    Task<CatalogItemResponseDto> GetAsync(int companyId,int itemId,CancellationToken cancellationToken = default);
    Task<CatalogItemResponseDto> CreateAsync(int companyId,int userId,CatalogItemType type,UpsertCatalogItemRequestDto request,CancellationToken cancellationToken = default);
    Task<CatalogItemResponseDto> UpdateAsync(int companyId,int userId,int itemId,UpsertCatalogItemRequestDto request,CancellationToken cancellationToken = default);
    Task SetStatusAsync(int companyId,int userId,int itemId,bool isActive,CancellationToken cancellationToken = default);
}
