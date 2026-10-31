import { computed, Injectable, signal } from '@angular/core';

import { CompanyResponseDto } from '../../features/organization/dto/company/company.dto';

const currentCompanyStorageKey = 'smart-erp-current-company-id';

@Injectable({ providedIn: 'root' })
export class CompanyContextService {
  private readonly selectedCompany = signal<CompanyResponseDto | null>(null);
  private readonly storedCompanyId = signal<number | null>(this.readStoredCompanyId());

  readonly currentCompany = this.selectedCompany.asReadonly();
  readonly currentCompanyId = computed(
    () => this.selectedCompany()?.id ?? this.storedCompanyId(),
  );
  readonly hasCurrentCompany = computed(() => this.currentCompanyId() !== null);

  selectCompany(company: CompanyResponseDto): void {
    this.selectedCompany.set(company);
    this.storedCompanyId.set(company.id);
    localStorage.setItem(currentCompanyStorageKey, String(company.id));
  }

  synchronize(companies: CompanyResponseDto[]): void {
    const selectedId = this.currentCompanyId();
    const selected = companies.find((company) => company.id === selectedId);

    if (selected) {
      this.selectCompany(selected);
      return;
    }

    const firstActiveCompany = companies.find((company) => company.isActive);
    if (firstActiveCompany) {
      this.selectCompany(firstActiveCompany);
    } else {
      this.clear();
    }
  }

  clear(): void {
    this.selectedCompany.set(null);
    this.storedCompanyId.set(null);
    localStorage.removeItem(currentCompanyStorageKey);
  }

  private readStoredCompanyId(): number | null {
    const value = Number(localStorage.getItem(currentCompanyStorageKey));
    return Number.isInteger(value) && value > 0 ? value : null;
  }
}
