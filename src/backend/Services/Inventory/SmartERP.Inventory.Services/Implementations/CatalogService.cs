using SmartERP.Inventory.Domain.Entities;
using SmartERP.Inventory.Domain.Enums;
using SmartERP.Inventory.Repositories.Interfaces;
using SmartERP.Inventory.Services.DTO;
using SmartERP.Inventory.Services.Interfaces;

namespace SmartERP.Inventory.Services.Implementations;

public sealed class CatalogService(ICatalogRepository repository) : ICatalogService
{
    public async Task<IReadOnlyList<CatalogItemResponseDto>> ListAsync(int companyId,CatalogItemType type,CancellationToken cancellationToken = default)
    {
        ValidateCompany(companyId);
        var items = await repository.ListAsync(companyId,type,cancellationToken);
        return items.Select(Map).ToArray();
    }

    public async Task<CatalogItemResponseDto> GetAsync(int companyId,int itemId,CancellationToken cancellationToken = default)
    {
        return Map(await GetRequiredAsync(companyId,itemId,cancellationToken));
    }

    public async Task<CatalogItemResponseDto> CreateAsync(int companyId,int userId,CatalogItemType type,UpsertCatalogItemRequestDto request,CancellationToken cancellationToken = default)
    {
        ValidateCompany(companyId);
        ValidateUser(userId);
        var normalized = Normalize(request);
        if (await repository.CodeExistsAsync(companyId,type,normalized.Code,null,cancellationToken))
        {
            throw new InvalidOperationException("A catalog item with this code already exists.");
        }

        var item = new CatalogItem
        {
            CompanyId = companyId,
            Type = type,
            Name = normalized.Name,
            Code = normalized.Code,
            Description = normalized.Description,
            RelatedItemId = normalized.RelatedItemId,
            Amount = normalized.Amount,
            Rate = normalized.Rate,
            CurrencyCode = normalized.CurrencyCode,
            MetadataJson = normalized.MetadataJson,
            IsActive = normalized.IsActive,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CreatedByUserId = userId
        };

        await repository.AddAsync(item,cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task<CatalogItemResponseDto> UpdateAsync(int companyId,int userId,int itemId,UpsertCatalogItemRequestDto request,CancellationToken cancellationToken = default)
    {
        ValidateUser(userId);
        var item = await GetRequiredAsync(companyId,itemId,cancellationToken);
        var normalized = Normalize(request);
        if (await repository.CodeExistsAsync(companyId,item.Type,normalized.Code,item.Id,cancellationToken))
        {
            throw new InvalidOperationException("A catalog item with this code already exists.");
        }

        item.Name = normalized.Name;
        item.Code = normalized.Code;
        item.Description = normalized.Description;
        item.RelatedItemId = normalized.RelatedItemId;
        item.Amount = normalized.Amount;
        item.Rate = normalized.Rate;
        item.CurrencyCode = normalized.CurrencyCode;
        item.MetadataJson = normalized.MetadataJson;
        item.IsActive = normalized.IsActive;
        item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        item.UpdatedByUserId = userId;
        await repository.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task SetStatusAsync(int companyId,int userId,int itemId,bool isActive,CancellationToken cancellationToken = default)
    {
        ValidateUser(userId);
        var item = await GetRequiredAsync(companyId,itemId,cancellationToken);
        item.IsActive = isActive;
        item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        item.UpdatedByUserId = userId;
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<CatalogItem> GetRequiredAsync(int companyId,int itemId,CancellationToken cancellationToken)
    {
        ValidateCompany(companyId);
        if (itemId <= 0) throw new ArgumentOutOfRangeException(nameof(itemId));
        return await repository.FindAsync(companyId,itemId,cancellationToken)
            ?? throw new KeyNotFoundException("Catalog item was not found.");
    }

    private static UpsertCatalogItemRequestDto Normalize(UpsertCatalogItemRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Name is required.",nameof(request));
        if (string.IsNullOrWhiteSpace(request.Code)) throw new ArgumentException("Code is required.",nameof(request));
        if (request.Name.Trim().Length > 160) throw new ArgumentException("Name must not exceed 160 characters.",nameof(request));
        if (request.Code.Trim().Length > 64) throw new ArgumentException("Code must not exceed 64 characters.",nameof(request));
        if (request.Amount < 0) throw new ArgumentOutOfRangeException(nameof(request),"Amount cannot be negative.");
        if (request.Rate is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(request),"Rate must be between 0 and 100.");
        return request with
        {
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Description = Optional(request.Description),
            CurrencyCode = Optional(request.CurrencyCode)?.ToUpperInvariant(),
            MetadataJson = Optional(request.MetadataJson)
        };
    }

    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static void ValidateCompany(int companyId) { if (companyId <= 0) throw new ArgumentOutOfRangeException(nameof(companyId)); }
    private static void ValidateUser(int userId) { if (userId <= 0) throw new UnauthorizedAccessException("Authenticated user identifier is invalid."); }
    private static CatalogItemResponseDto Map(CatalogItem item) => new(item.Id,item.CompanyId,item.Type.ToString(),item.Name,item.Code,item.Description,item.RelatedItemId,item.Amount,item.Rate,item.CurrencyCode,item.MetadataJson,item.IsActive,item.CreatedAtUtc,item.UpdatedAtUtc);
}
