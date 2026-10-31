using SmartERP.Organization.Domain.Common;

namespace SmartERP.Organization.Domain.Entities;

public sealed class CompanySettings : AuditableEntity
{
    public int CompanyId { get; set; }

    public string DefaultLanguage { get; set; } = "en";

    public string TimeZoneId { get; set; } = "Asia/Amman";

    public string DateFormat { get; set; } = "yyyy-MM-dd";

    public string TimeFormat { get; set; } = "HH:mm";

    public int NumberDecimalPlaces { get; set; } = 2;

    public int QuantityDecimalPlaces { get; set; } = 3;

    public string SalesInvoicePrefix { get; set; } = "INV";

    public string PurchaseOrderPrefix { get; set; } = "PO";

    public string? LogoUrl { get; set; }

    public Company Company { get; set; } = null!;
}