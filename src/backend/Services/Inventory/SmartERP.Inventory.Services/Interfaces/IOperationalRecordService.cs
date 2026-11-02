using SmartERP.Inventory.Domain.Enums;
using SmartERP.Inventory.Services.DTO;

namespace SmartERP.Inventory.Services.Interfaces;

public interface IOperationalRecordService
{
    Task<IReadOnlyList<OperationalRecordResponseDto>> ListAsync(int companyId,OperationalRecordType type,CancellationToken cancellationToken = default);
    Task<OperationalRecordResponseDto> GetAsync(int companyId,long recordId,CancellationToken cancellationToken = default);
    Task<OperationalRecordResponseDto> CreateAsync(int companyId,int userId,OperationalRecordType type,UpsertOperationalRecordRequestDto request,CancellationToken cancellationToken = default);
    Task<OperationalRecordResponseDto> UpdateAsync(int companyId,int userId,long recordId,UpsertOperationalRecordRequestDto request,CancellationToken cancellationToken = default);
    Task SetStatusAsync(int companyId,int userId,long recordId,bool isActive,CancellationToken cancellationToken = default);
}
