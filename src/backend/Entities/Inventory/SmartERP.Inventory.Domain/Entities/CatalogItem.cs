using SmartERP.Inventory.Domain.Enums;

namespace SmartERP.Inventory.Domain.Entities;

public sealed class CatalogItem
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public CatalogItemType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? RelatedItemId { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Rate { get; set; }
    public string? CurrencyCode { get; set; }
    public string? MetadataJson { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
    public int? UpdatedByUserId { get; set; }
}
