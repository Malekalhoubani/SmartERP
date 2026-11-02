using SmartERP.Inventory.Domain.Entities;
using SmartERP.Inventory.Domain.Enums;
using SmartERP.Inventory.Repositories.Interfaces;
using SmartERP.Inventory.Services.DTO;
using SmartERP.Inventory.Services.Interfaces;

namespace SmartERP.Inventory.Services.Implementations;

public sealed class OperationalRecordService(IOperationalRecordRepository repository) : IOperationalRecordService
{
    public async Task<IReadOnlyList<OperationalRecordResponseDto>> ListAsync(int companyId,OperationalRecordType type,CancellationToken cancellationToken = default)
    {
        ValidateCompany(companyId);
        return (await repository.ListAsync(companyId,type,cancellationToken)).Select(Map).ToArray();
    }

    public async Task<OperationalRecordResponseDto> GetAsync(int companyId,long recordId,CancellationToken cancellationToken = default) =>
        Map(await GetRequiredAsync(companyId,recordId,cancellationToken));

    public async Task<OperationalRecordResponseDto> CreateAsync(int companyId,int userId,OperationalRecordType type,UpsertOperationalRecordRequestDto request,CancellationToken cancellationToken = default)
    {
        ValidateCompany(companyId);
        ValidateUser(userId);
        var normalized = Normalize(request);
        if (await repository.CodeExistsAsync(companyId,type,normalized.Code,null,cancellationToken))
            throw new InvalidOperationException("A record with this code already exists.");

        var record = new OperationalRecord
        {
            CompanyId = companyId,
            Type = type,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CreatedByUserId = userId
        };
        Apply(record,normalized);
        await repository.AddAsync(record,cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(record);
    }

    public async Task<OperationalRecordResponseDto> UpdateAsync(int companyId,int userId,long recordId,UpsertOperationalRecordRequestDto request,CancellationToken cancellationToken = default)
    {
        ValidateUser(userId);
        var record = await GetRequiredAsync(companyId,recordId,cancellationToken);
        var normalized = Normalize(request);
        if (await repository.CodeExistsAsync(companyId,record.Type,normalized.Code,record.Id,cancellationToken))
            throw new InvalidOperationException("A record with this code already exists.");
        Apply(record,normalized);
        record.UpdatedAtUtc = DateTimeOffset.UtcNow;
        record.UpdatedByUserId = userId;
        await repository.SaveChangesAsync(cancellationToken);
        return Map(record);
    }

    public async Task SetStatusAsync(int companyId,int userId,long recordId,bool isActive,CancellationToken cancellationToken = default)
    {
        ValidateUser(userId);
        var record = await GetRequiredAsync(companyId,recordId,cancellationToken);
        record.IsActive = isActive;
        record.UpdatedAtUtc = DateTimeOffset.UtcNow;
        record.UpdatedByUserId = userId;
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<OperationalRecord> GetRequiredAsync(int companyId,long recordId,CancellationToken cancellationToken)
    {
        ValidateCompany(companyId);
        if (recordId <= 0) throw new ArgumentOutOfRangeException(nameof(recordId));
        return await repository.FindAsync(companyId,recordId,cancellationToken)
            ?? throw new KeyNotFoundException("Operational record was not found.");
    }

    private static UpsertOperationalRecordRequestDto Normalize(UpsertOperationalRecordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Name is required.",nameof(request));
        if (string.IsNullOrWhiteSpace(request.Code)) throw new ArgumentException("Code is required.",nameof(request));
        if (request.Quantity < 0 || request.ReservedQuantity < 0 || request.UnitCost < 0 || request.MinimumStockLevel < 0)
            throw new ArgumentOutOfRangeException(nameof(request),"Quantity and financial values cannot be negative.");
        return request with
        {
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Description = Optional(request.Description),
            ReferenceNumber = Optional(request.ReferenceNumber),
            CurrencyCode = Optional(request.CurrencyCode)?.ToUpperInvariant(),
            Status = Optional(request.Status) ?? "Draft",
            BatchNumber = Optional(request.BatchNumber),
            SerialNumber = Optional(request.SerialNumber),
            MetadataJson = Optional(request.MetadataJson)
        };
    }

    private static void Apply(OperationalRecord record,UpsertOperationalRecordRequestDto request)
    {
        record.Name = request.Name;
        record.Code = request.Code;
        record.Description = request.Description;
        record.ReferenceNumber = request.ReferenceNumber;
        record.ProductId = request.ProductId;
        record.WarehouseId = request.WarehouseId;
        record.DestinationWarehouseId = request.DestinationWarehouseId;
        record.SupplierId = request.SupplierId;
        record.Quantity = request.Quantity;
        record.ReservedQuantity = request.ReservedQuantity;
        record.UnitCost = request.UnitCost;
        record.MinimumStockLevel = request.MinimumStockLevel;
        record.CurrencyCode = request.CurrencyCode;
        record.Status = request.Status ?? "Draft";
        record.BatchNumber = request.BatchNumber;
        record.SerialNumber = request.SerialNumber;
        record.DocumentDateUtc = request.DocumentDateUtc;
        record.ExpectedDateUtc = request.ExpectedDateUtc;
        record.ExpirationDateUtc = request.ExpirationDateUtc;
        record.MetadataJson = request.MetadataJson;
        record.IsActive = request.IsActive;
    }

    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static void ValidateCompany(int companyId) { if (companyId <= 0) throw new ArgumentOutOfRangeException(nameof(companyId)); }
    private static void ValidateUser(int userId) { if (userId <= 0) throw new UnauthorizedAccessException("Authenticated user identifier is invalid."); }
    private static OperationalRecordResponseDto Map(OperationalRecord record) => new(
        record.Id,record.CompanyId,record.Type.ToString(),record.Name,record.Code,record.Description,record.ReferenceNumber,
        record.ProductId,record.WarehouseId,record.DestinationWarehouseId,record.SupplierId,record.Quantity,
        record.ReservedQuantity,record.UnitCost,record.MinimumStockLevel,record.CurrencyCode,record.Status,
        record.BatchNumber,record.SerialNumber,record.DocumentDateUtc,record.ExpectedDateUtc,record.ExpirationDateUtc,
        record.MetadataJson,record.IsActive,record.CreatedAtUtc,record.UpdatedAtUtc);
}
