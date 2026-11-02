using SmartERP.Inventory.API.Authentication;
using SmartERP.Inventory.Domain.Enums;
using SmartERP.Inventory.Services.DTO;
using SmartERP.Inventory.Services.Interfaces;

namespace SmartERP.Inventory.API.Endpoints;

public static class OperationalRecordEndpoints
{
    public static IEndpointRouteBuilder MapOperationalRecordEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/operations").RequireAuthorization().AddEndpointFilter<CompanyAccessFilter>().WithTags("Inventory and Purchasing");
        group.MapGet("/{type}", ListAsync);
        group.MapGet("/{type}/{recordId:long}", GetAsync);
        group.MapPost("/{type}", CreateAsync);
        group.MapPut("/{type}/{recordId:long}", UpdateAsync);
        group.MapPatch("/{type}/{recordId:long}/activate", (string type, long recordId, IOperationalRecordService service, IRequestContext context, CancellationToken token) => SetStatusAsync(type, recordId, true, service, context, token));
        group.MapPatch("/{type}/{recordId:long}/deactivate", (string type, long recordId, IOperationalRecordService service, IRequestContext context, CancellationToken token) => SetStatusAsync(type, recordId, false, service, context, token));
        return endpoints;
    }

    private static async Task<IResult> ListAsync(string type, IOperationalRecordService service, IRequestContext context, CancellationToken token)
    {
        if (!TryType(type, out var recordType)) return InvalidType();
        return Results.Ok(await service.ListAsync(context.CompanyId, recordType, token));
    }

    private static async Task<IResult> GetAsync(string type, long recordId, IOperationalRecordService service, IRequestContext context, CancellationToken token)
    {
        if (!TryType(type, out _)) return InvalidType();
        return Results.Ok(await service.GetAsync(context.CompanyId, recordId, token));
    }

    private static async Task<IResult> CreateAsync(string type, UpsertOperationalRecordRequestDto request, IOperationalRecordService service, IRequestContext context, CancellationToken token)
    {
        if (!TryType(type, out var recordType)) return InvalidType();
        var result = await service.CreateAsync(context.CompanyId, context.UserId, recordType, request, token);
        return Results.Created($"/api/operations/{type}/{result.Id}", result);
    }

    private static async Task<IResult> UpdateAsync(string type, long recordId, UpsertOperationalRecordRequestDto request, IOperationalRecordService service, IRequestContext context, CancellationToken token)
    {
        if (!TryType(type, out _)) return InvalidType();
        return Results.Ok(await service.UpdateAsync(context.CompanyId, context.UserId, recordId, request, token));
    }

    private static async Task<IResult> SetStatusAsync(string type, long recordId, bool isActive, IOperationalRecordService service, IRequestContext context, CancellationToken token)
    {
        if (!TryType(type, out _)) return InvalidType();
        await service.SetStatusAsync(context.CompanyId, context.UserId, recordId, isActive, token);
        return Results.NoContent();
    }

    private static bool TryType(string value, out OperationalRecordType type)
    {
        type = value.ToLowerInvariant() switch
        {
            "stock-balances" => OperationalRecordType.StockBalance,
            "stock-movements" => OperationalRecordType.StockMovement,
            "transfers" => OperationalRecordType.WarehouseTransfer,
            "stock-adjustments" => OperationalRecordType.StockAdjustment,
            "reservations" => OperationalRecordType.Reservation,
            "batch-lot-tracking" => OperationalRecordType.BatchLot,
            "serial-numbers" => OperationalRecordType.SerialNumber,
            "expiration-dates" => OperationalRecordType.ExpirationDate,
            "minimum-stock-alerts" => OperationalRecordType.MinimumStockAlert,
            "suppliers" => OperationalRecordType.Supplier,
            "purchase-requests" => OperationalRecordType.PurchaseRequest,
            "purchase-orders" => OperationalRecordType.PurchaseOrder,
            "goods-receipts" => OperationalRecordType.GoodsReceipt,
            "purchase-returns" => OperationalRecordType.PurchaseReturn,
            "supplier-invoices" => OperationalRecordType.SupplierInvoice,
            "approval-workflow" => OperationalRecordType.ApprovalWorkflow,
            _ => 0
        };
        return Enum.IsDefined(type);
    }

    private static IResult InvalidType() => Results.ValidationProblem(new Dictionary<string, string[]> { ["type"] = ["Unsupported operation type."] });
}
