namespace SmartERP.Inventory.Domain.Enums;

public enum OperationalRecordType
{
    StockBalance = 1,
    StockMovement = 2,
    WarehouseTransfer = 3,
    StockAdjustment = 4,
    Reservation = 5,
    BatchLot = 6,
    SerialNumber = 7,
    ExpirationDate = 8,
    MinimumStockAlert = 9,
    Supplier = 10,
    PurchaseRequest = 11,
    PurchaseOrder = 12,
    GoodsReceipt = 13,
    PurchaseReturn = 14,
    SupplierInvoice = 15,
    ApprovalWorkflow = 16
}
