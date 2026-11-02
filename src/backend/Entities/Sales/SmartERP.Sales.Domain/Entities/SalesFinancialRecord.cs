using SmartERP.Sales.Domain.Enums;
namespace SmartERP.Sales.Domain.Entities;
public sealed class SalesFinancialRecord
{
    public long Id { get; set; } public int CompanyId { get; set; } public SalesFinancialRecordType Type { get; set; }
    public string Name { get; set; }=string.Empty; public string Code { get; set; }=string.Empty; public string? Description { get; set; } public string? ReferenceNumber { get; set; }
    public long? CustomerId { get; set; } public long? AccountId { get; set; } public long? RelatedDocumentId { get; set; }
    public decimal? Quantity { get; set; } public decimal? Subtotal { get; set; } public decimal? TaxAmount { get; set; } public decimal? DiscountAmount { get; set; } public decimal? TotalAmount { get; set; } public decimal? Balance { get; set; } public decimal? Debit { get; set; } public decimal? Credit { get; set; }
    public string? CurrencyCode { get; set; } public string Status { get; set; }="Draft";
    public DateTimeOffset? DocumentDateUtc { get; set; } public DateTimeOffset? DueDateUtc { get; set; } public DateTimeOffset? PeriodStartUtc { get; set; } public DateTimeOffset? PeriodEndUtc { get; set; }
    public string? MetadataJson { get; set; } public bool IsActive { get; set; }=true; public DateTimeOffset CreatedAtUtc { get; set; } public int CreatedByUserId { get; set; } public DateTimeOffset? UpdatedAtUtc { get; set; } public int? UpdatedByUserId { get; set; }
}
