namespace SmartERP.Organization.Domain.Common;

public abstract class AuditableEntity
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public int? UpdatedByUserId { get; set; }
}