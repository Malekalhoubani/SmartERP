using SmartERP.Inventory.Domain.Enums;

namespace SmartERP.Inventory.Domain.Entities;

public sealed class OperationalRecord
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public OperationalRecordType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ReferenceNumber { get; set; }
    public int? ProductId { get; set; }
    public int? WarehouseId { get; set; }
    public int? DestinationWarehouseId { get; set; }
    public long? SupplierId { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? ReservedQuantity { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? MinimumStockLevel { get; set; }
    public string? CurrencyCode { get; set; }
    public string Status { get; set; } = "Draft";
    public string? BatchNumber { get; set; }
    public string? SerialNumber { get; set; }
    public DateTimeOffset? DocumentDateUtc { get; set; }
    public DateTimeOffset? ExpectedDateUtc { get; set; }
    public DateTimeOffset? ExpirationDateUtc { get; set; }
    public string? MetadataJson { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
    public int? UpdatedByUserId { get; set; }
}
