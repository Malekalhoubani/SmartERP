using SmartERP.Organization.Domain.Common;
using SmartERP.Organization.Domain.Enums;

namespace SmartERP.Organization.Domain.Entities;

public sealed class Tax : AuditableEntity
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public decimal RatePercentage { get; set; }

    public TaxCalculationMethod CalculationMethod { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
}