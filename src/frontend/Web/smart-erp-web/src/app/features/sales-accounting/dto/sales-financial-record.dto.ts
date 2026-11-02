export type SalesFinancialRecordType =
  | 'customers' | 'quotations' | 'sales-orders' | 'delivery-notes'
  | 'sales-invoices' | 'sales-returns' | 'discounts' | 'order-status-workflow'
  | 'payment-methods' | 'receipts-payments' | 'chart-of-accounts' | 'journal-entries'
  | 'accounts-receivable' | 'accounts-payable' | 'tax-entries' | 'financial-periods';

export interface SalesFinancialRecordDto {
  id: number; companyId: number; type: string; name: string; code: string;
  description: string | null; referenceNumber: string | null;
  customerId: number | null; accountId: number | null; relatedDocumentId: number | null;
  quantity: number | null; subtotal: number | null; taxAmount: number | null;
  discountAmount: number | null; totalAmount: number | null; balance: number | null;
  debit: number | null; credit: number | null; currencyCode: string | null;
  status: string; documentDateUtc: string | null; dueDateUtc: string | null;
  periodStartUtc: string | null; periodEndUtc: string | null; metadataJson: string | null;
  isActive: boolean; createdAtUtc: string; updatedAtUtc: string | null;
}

export type UpsertSalesFinancialRecordDto = Omit<SalesFinancialRecordDto,
  'id' | 'companyId' | 'type' | 'createdAtUtc' | 'updatedAtUtc'>;
