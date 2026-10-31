namespace SmartERP.Inventory.Services.DTO;

public sealed record UpsertCatalogItemRequestDto(
    string Name,
    string Code,
    string? Description,
    int? RelatedItemId,
    decimal? Amount,
    decimal? Rate,
    string? CurrencyCode,
    string? MetadataJson,
    bool IsActive = true);

public sealed record CatalogItemResponseDto(
    int Id,
    int CompanyId,
    string Type,
    string Name,
    string Code,
    string? Description,
    int? RelatedItemId,
    decimal? Amount,
    decimal? Rate,
    string? CurrencyCode,
    string? MetadataJson,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
