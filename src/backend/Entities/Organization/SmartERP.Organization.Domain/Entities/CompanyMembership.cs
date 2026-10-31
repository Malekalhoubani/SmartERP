using SmartERP.Organization.Domain.Common;
using SmartERP.Organization.Domain.Enums;

namespace SmartERP.Organization.Domain.Entities;

public sealed class CompanyMembership : AuditableEntity
{
    public int CompanyId { get; set; }

    public int UserId { get; set; }

    public CompanyMembershipRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
}