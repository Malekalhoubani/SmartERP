using SmartERP.Organization.Domain.Common;

namespace SmartERP.Organization.Domain.Entities;

public sealed class Company : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string LegalName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? RegistrationNumber { get; set; }

    public string? TaxNumber { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;
    public CompanySettings? Settings { get; set; }
    public ICollection<CompanyMembership> Memberships { get; set; } =new List<CompanyMembership>();
    public ICollection<Branch> Branches { get; set; } =new List<Branch>();
    public ICollection<CompanyCurrency> Currencies { get; set; } =new List<CompanyCurrency>();
    public ICollection<Tax> Taxes { get; set; } =new List<Tax>();

    public ICollection<FiscalYear> FiscalYears { get; set; } = new List<FiscalYear>();
}