using SmartERP.Organization.Domain.Common;

namespace SmartERP.Organization.Domain.Entities;

public sealed class CompanyCurrency : AuditableEntity
{
    public int CompanyId { get; set; }

    public int CurrencyId { get; set; }

    public bool IsBaseCurrency { get; set; }

    public decimal ExchangeRateToBase { get; set; } = 1m;

    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;

    public Currency Currency { get; set; } = null!;
}