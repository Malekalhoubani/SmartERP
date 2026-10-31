using SmartERP.Organization.Domain.Common;

namespace SmartERP.Organization.Domain.Entities;

public sealed class Branch : AuditableEntity
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? StateOrProvince { get; set; }

    public string? PostalCode { get; set; }

    public string? CountryCode { get; set; }

    public bool IsMainBranch { get; set; }

    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
}