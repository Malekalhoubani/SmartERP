using SmartERP.Organization.Domain.Common;
using SmartERP.Organization.Domain.Enums;

namespace SmartERP.Organization.Domain.Entities;

public sealed class FiscalYear : AuditableEntity
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public FiscalYearStatus Status { get; set; } =
        FiscalYearStatus.Draft;

    public bool IsCurrent { get; set; }

    public DateTimeOffset? ClosedAtUtc { get; set; }

    public int? ClosedByUserId { get; set; }

    public Company Company { get; set; } = null!;
}