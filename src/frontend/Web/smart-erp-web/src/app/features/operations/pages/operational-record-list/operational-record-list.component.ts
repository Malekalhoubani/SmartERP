import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { OperationalRecordService } from '../../../../core/services/operational-record.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OperationalRecordDto, OperationalRecordType, UpsertOperationalRecordDto } from '../../dto/operational-record.dto';

const supportedTypes: OperationalRecordType[] = [
  'stock-balances','stock-movements','transfers','stock-adjustments','reservations',
  'batch-lot-tracking','serial-numbers','expiration-dates','minimum-stock-alerts',
  'suppliers','purchase-requests','purchase-orders','goods-receipts','purchase-returns',
  'supplier-invoices','approval-workflow',
];

@Component({
  selector: 'app-operational-record-list',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './operational-record-list.component.html',
  styleUrl: './operational-record-list.component.scss',
})
export class OperationalRecordListComponent implements OnInit {
  private readonly service = inject(OperationalRecordService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly toast = inject(ToastService);

  readonly translation = inject(TranslationService);
  readonly type = signal<OperationalRecordType>('stock-balances');
  readonly records = signal<OperationalRecordDto[]>([]);
  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly editingId = signal<number | null>(null);
  readonly showForm = signal(false);

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required,Validators.maxLength(200)]], code: ['', [Validators.required,Validators.maxLength(64)]],
    description: [''], referenceNumber: [''], productId: [''], warehouseId: [''], destinationWarehouseId: [''], supplierId: [''],
    quantity: [''], reservedQuantity: [''], unitCost: [''], minimumStockLevel: [''], currencyCode: [''], status: ['Draft'],
    batchNumber: [''], serialNumber: [''], documentDateUtc: [''], expectedDateUtc: [''], expirationDateUtc: [''], metadataJson: [''], isActive: [true],
  });

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      const type = params.get('type') as OperationalRecordType;
      if (!supportedTypes.includes(type)) return;
      this.type.set(type); this.cancelEdit(); this.load();
    });
  }

  title(): string { return this.translation.t('menu',this.menuKey()); }
  sectionTitle(): string { return this.translation.t('menu',this.isPurchasing() ? 'purchasing' : 'inventoryManagement'); }
  isPurchasing(): boolean { return supportedTypes.indexOf(this.type()) >= 9; }
  needsProduct(): boolean { return !['suppliers','approval-workflow'].includes(this.type()); }
  needsWarehouse(): boolean { return !['suppliers','purchase-requests','purchase-orders','supplier-invoices','approval-workflow'].includes(this.type()); }
  needsDestination(): boolean { return this.type() === 'transfers'; }
  needsSupplier(): boolean { return ['purchase-requests','purchase-orders','goods-receipts','purchase-returns','supplier-invoices'].includes(this.type()); }
  needsQuantity(): boolean { return !['suppliers','approval-workflow'].includes(this.type()); }
  needsCost(): boolean { return ['stock-balances','stock-movements','purchase-orders','goods-receipts','purchase-returns','supplier-invoices'].includes(this.type()); }
  needsBatch(): boolean { return this.type() === 'batch-lot-tracking'; }
  needsSerial(): boolean { return this.type() === 'serial-numbers'; }
  needsExpiration(): boolean { return ['expiration-dates','batch-lot-tracking'].includes(this.type()); }
  needsMinimum(): boolean { return this.type() === 'minimum-stock-alerts'; }

  openCreate(): void { this.editingId.set(null); this.form.reset({ status: 'Draft',isActive: true }); this.showForm.set(true); }
  cancelEdit(): void { this.showForm.set(false); this.editingId.set(null); this.form.reset({ status: 'Draft',isActive: true }); }

  edit(record: OperationalRecordDto): void {
    this.editingId.set(record.id);
    this.form.patchValue({
      name: record.name,code: record.code,description: record.description ?? '',referenceNumber: record.referenceNumber ?? '',
      productId: this.text(record.productId),warehouseId: this.text(record.warehouseId),destinationWarehouseId: this.text(record.destinationWarehouseId),supplierId: this.text(record.supplierId),
      quantity: this.text(record.quantity),reservedQuantity: this.text(record.reservedQuantity),unitCost: this.text(record.unitCost),minimumStockLevel: this.text(record.minimumStockLevel),
      currencyCode: record.currencyCode ?? '',status: record.status,batchNumber: record.batchNumber ?? '',serialNumber: record.serialNumber ?? '',
      documentDateUtc: this.date(record.documentDateUtc),expectedDateUtc: this.date(record.expectedDateUtc),expirationDateUtc: this.date(record.expirationDateUtc),
      metadataJson: record.metadataJson ?? '',isActive: record.isActive,
    });
    this.showForm.set(true);
  }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const value = this.form.getRawValue();
    const request: UpsertOperationalRecordDto = {
      name: value.name.trim(),code: value.code.trim().toUpperCase(),description: this.optional(value.description),referenceNumber: this.optional(value.referenceNumber),
      productId: this.number(value.productId),warehouseId: this.number(value.warehouseId),destinationWarehouseId: this.number(value.destinationWarehouseId),supplierId: this.number(value.supplierId),
      quantity: this.number(value.quantity),reservedQuantity: this.number(value.reservedQuantity),unitCost: this.number(value.unitCost),minimumStockLevel: this.number(value.minimumStockLevel),
      currencyCode: this.optional(value.currencyCode)?.toUpperCase() ?? null,status: value.status.trim() || 'Draft',batchNumber: this.optional(value.batchNumber),serialNumber: this.optional(value.serialNumber),
      documentDateUtc: this.iso(value.documentDateUtc),expectedDateUtc: this.iso(value.expectedDateUtc),expirationDateUtc: this.iso(value.expirationDateUtc),metadataJson: this.optional(value.metadataJson),isActive: value.isActive,
    };
    const editingId = this.editingId();
    const operation = editingId ? this.service.update(this.type(),editingId,request) : this.service.create(this.type(),request);
    this.isSaving.set(true);
    operation.pipe(finalize(() => this.isSaving.set(false))).subscribe({ next: saved => {
      this.records.update(records => editingId ? records.map(record => record.id === saved.id ? saved : record) : [saved,...records]);
      this.toast.success(this.translation.t('alerts',editingId ? 'operationUpdated' : 'operationCreated',{ name: saved.name }));
      this.cancelEdit();
    }});
  }

  toggle(record: OperationalRecordDto): void {
    this.service.setActive(this.type(),record.id,!record.isActive).subscribe({ next: () => {
      this.records.update(records => records.map(current => current.id === record.id ? { ...current,isActive: !current.isActive } : current));
      this.toast.success(this.translation.t('alerts','operationStatusChanged',{ name: record.name }));
    }});
  }

  private load(): void { this.isLoading.set(true); this.service.list(this.type()).pipe(finalize(() => this.isLoading.set(false))).subscribe({ next: records => this.records.set(records) }); }
  private optional(value: string): string | null { const result = value.trim(); return result || null; }
  private number(value: string): number | null { if (!value.trim()) return null; const result = Number(value); return Number.isFinite(result) ? result : null; }
  private text(value: number | null): string { return value === null ? '' : String(value); }
  private date(value: string | null): string { return value ? value.slice(0,10) : ''; }
  private iso(value: string): string | null { return value ? new Date(`${value}T00:00:00Z`).toISOString() : null; }
  private menuKey(): string { return ({
    'stock-balances':'stockBalances','stock-movements':'stockMovements','transfers':'transfers','stock-adjustments':'stockAdjustments','reservations':'reservations',
    'batch-lot-tracking':'batchLotTracking','serial-numbers':'serialNumbers','expiration-dates':'expirationDates','minimum-stock-alerts':'minimumStockAlerts',
    'suppliers':'suppliers','purchase-requests':'purchaseRequests','purchase-orders':'purchaseOrders','goods-receipts':'goodsReceipts','purchase-returns':'purchaseReturns',
    'supplier-invoices':'supplierInvoices','approval-workflow':'approvalWorkflow',
  } as const)[this.type()]; }
}
