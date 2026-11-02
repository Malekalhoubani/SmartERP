export type OperationalRecordType =
  | 'stock-balances' | 'stock-movements' | 'transfers' | 'stock-adjustments'
  | 'reservations' | 'batch-lot-tracking' | 'serial-numbers' | 'expiration-dates'
  | 'minimum-stock-alerts' | 'suppliers' | 'purchase-requests' | 'purchase-orders'
  | 'goods-receipts' | 'purchase-returns' | 'supplier-invoices' | 'approval-workflow';

export interface OperationalRecordDto {
  id: number;
  companyId: number;
  type: string;
  name: string;
  code: string;
  description: string | null;
  referenceNumber: string | null;
  productId: number | null;
  warehouseId: number | null;
  destinationWarehouseId: number | null;
  supplierId: number | null;
  quantity: number | null;
  reservedQuantity: number | null;
  unitCost: number | null;
  minimumStockLevel: number | null;
  currencyCode: string | null;
  status: string;
  batchNumber: string | null;
  serialNumber: string | null;
  documentDateUtc: string | null;
  expectedDateUtc: string | null;
  expirationDateUtc: string | null;
  metadataJson: string | null;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}

export type UpsertOperationalRecordDto = Omit<OperationalRecordDto,
  'id' | 'companyId' | 'type' | 'createdAtUtc' | 'updatedAtUtc'>;
