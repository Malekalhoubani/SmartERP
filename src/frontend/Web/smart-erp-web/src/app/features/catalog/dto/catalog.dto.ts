export type CatalogType =
  | 'products'
  | 'categories'
  | 'units-of-measure'
  | 'brands'
  | 'barcodes'
  | 'product-variants'
  | 'pricing'
  | 'tax-configuration';

export interface CatalogItemDto {
  id: number;
  companyId: number;
  type: string;
  name: string;
  code: string;
  description: string | null;
  relatedItemId: number | null;
  amount: number | null;
  rate: number | null;
  currencyCode: string | null;
  metadataJson: string | null;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}

export interface UpsertCatalogItemDto {
  name: string;
  code: string;
  description: string | null;
  relatedItemId: number | null;
  amount: number | null;
  rate: number | null;
  currencyCode: string | null;
  metadataJson: string | null;
  isActive: boolean;
}
