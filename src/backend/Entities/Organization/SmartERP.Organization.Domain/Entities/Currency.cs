using SmartERP.Organization.Domain.Common;

namespace SmartERP.Organization.Domain.Entities;

public sealed class Currency : AuditableEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    public int DecimalPlaces { get; set; } = 2;

    public bool IsActive { get; set; } = true;

    public ICollection<CompanyCurrency> Companies { get; set; } =new List<CompanyCurrency>();
}