import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { CatalogService } from '../../../../core/services/catalog.service';
import { ToastService } from '../../../../core/services/toast.service';
import { CatalogItemDto, CatalogType, UpsertCatalogItemDto } from '../../dto/catalog.dto';

const supportedTypes: CatalogType[] = ['products','categories','units-of-measure','brands','barcodes','product-variants','pricing','tax-configuration'];

@Component({
  selector: 'app-catalog-list',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './catalog-list.component.html',
  styleUrl: './catalog-list.component.scss',
})
export class CatalogListComponent implements OnInit {
  private readonly service = inject(CatalogService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly toast = inject(ToastService);

  readonly translation = inject(TranslationService);
  readonly type = signal<CatalogType>('products');
  readonly items = signal<CatalogItemDto[]>([]);
  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly editingId = signal<number | null>(null);
  readonly showForm = signal(false);

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required,Validators.maxLength(160)]],
    code: ['', [Validators.required,Validators.maxLength(64)]],
    description: ['', Validators.maxLength(1000)],
    relatedItemId: [''],
    amount: [''],
    rate: [''],
    currencyCode: [''],
    metadataJson: [''],
    isActive: [true],
  });

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const type = params.get('type') as CatalogType;
      if (!supportedTypes.includes(type)) return;
      this.type.set(type);
      this.cancelEdit();
      this.load();
    });
  }

  title(): string { return this.translation.t('menu',this.menuKey()); }
  showsAmount(): boolean { return this.type() === 'pricing'; }
  showsRate(): boolean { return this.type() === 'tax-configuration'; }
  showsRelatedItem(): boolean { return ['barcodes','product-variants','pricing'].includes(this.type()); }

  openCreate(): void { this.editingId.set(null); this.form.reset({ isActive: true }); this.showForm.set(true); }

  edit(item: CatalogItemDto): void {
    this.editingId.set(item.id);
    this.form.patchValue({
      name: item.name, code: item.code, description: item.description ?? '',
      relatedItemId: item.relatedItemId?.toString() ?? '', amount: item.amount?.toString() ?? '',
      rate: item.rate?.toString() ?? '', currencyCode: item.currencyCode ?? '',
      metadataJson: item.metadataJson ?? '', isActive: item.isActive,
    });
    this.showForm.set(true);
  }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const value = this.form.getRawValue();
    const request: UpsertCatalogItemDto = {
      name: value.name.trim(), code: value.code.trim().toUpperCase(),
      description: this.optional(value.description), relatedItemId: this.numberOrNull(value.relatedItemId),
      amount: this.numberOrNull(value.amount), rate: this.numberOrNull(value.rate),
      currencyCode: this.optional(value.currencyCode)?.toUpperCase() ?? null,
      metadataJson: this.optional(value.metadataJson), isActive: value.isActive,
    };
    const editingId = this.editingId();
    const operation = editingId ? this.service.update(this.type(),editingId,request) : this.service.create(this.type(),request);
    this.isSaving.set(true);
    operation.pipe(finalize(() => this.isSaving.set(false))).subscribe({ next: (saved) => {
      this.items.update((items) => editingId ? items.map(item => item.id === saved.id ? saved : item) : [...items,saved].sort((a,b) => a.name.localeCompare(b.name)));
      this.toast.success(this.translation.t('alerts',editingId ? 'catalogUpdated' : 'catalogCreated',{ name: saved.name }));
      this.cancelEdit();
    }});
  }

  toggleStatus(item: CatalogItemDto): void {
    this.service.setStatus(this.type(),item.id,!item.isActive).subscribe({ next: () => {
      this.items.update(items => items.map(current => current.id === item.id ? { ...current,isActive: !current.isActive } : current));
      this.toast.success(this.translation.t('alerts','catalogStatusChanged',{ name: item.name }));
    }});
  }

  cancelEdit(): void { this.showForm.set(false); this.editingId.set(null); this.form.reset({ isActive: true }); }

  private load(): void { this.isLoading.set(true); this.service.list(this.type()).pipe(finalize(() => this.isLoading.set(false))).subscribe({ next: items => this.items.set(items) }); }
  private optional(value: string): string | null { const result = value.trim(); return result || null; }
  private numberOrNull(value: string): number | null { if (!value.trim()) return null; const parsed = Number(value); return Number.isFinite(parsed) ? parsed : null; }
  private menuKey(): string { return ({ products:'products',categories:'categories','units-of-measure':'unitsOfMeasure',brands:'brands',barcodes:'barcodes','product-variants':'productVariants',pricing:'pricing','tax-configuration':'taxConfiguration' } as const)[this.type()]; }
}
