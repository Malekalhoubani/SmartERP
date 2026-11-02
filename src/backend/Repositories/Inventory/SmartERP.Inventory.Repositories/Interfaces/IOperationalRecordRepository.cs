using SmartERP.Inventory.Domain.Entities;
using SmartERP.Inventory.Domain.Enums;

namespace SmartERP.Inventory.Repositories.Interfaces;

public interface IOperationalRecordRepository
{
    Task<IReadOnlyList<OperationalRecord>> ListAsync(int companyId,OperationalRecordType type,CancellationToken cancellationToken = default);
    Task<OperationalRecord?> FindAsync(int companyId,long recordId,CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(int companyId,OperationalRecordType type,string code,long? excludedId = null,CancellationToken cancellationToken = default);
    Task AddAsync(OperationalRecord record,CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
