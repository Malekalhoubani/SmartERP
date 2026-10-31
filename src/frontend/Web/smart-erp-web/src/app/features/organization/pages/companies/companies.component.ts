import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { CompanyContextService } from '../../../../core/services/company-context.service';
import { OrganizationService } from '../../../../core/services/organization.service';
import { ToastService } from '../../../../core/services/toast.service';
import { CompanyResponseDto } from '../../dto/company/company.dto';

@Component({
  selector: 'app-companies',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './companies.component.html',
  styleUrl: './companies.component.scss',
})
export class CompaniesComponent implements OnInit {
  private readonly organizationService = inject(OrganizationService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  readonly translation = inject(TranslationService);
  readonly companyContext = inject(CompanyContextService);
  readonly companies = signal<CompanyResponseDto[]>([]);
  readonly isLoading = signal(true);
  readonly pendingCompanyId = signal<number | null>(null);

  ngOnInit(): void {
    this.loadCompanies();
  }

  selectCompany(company: CompanyResponseDto): void {
    if (!company.isActive) {
      return;
    }

    this.companyContext.selectCompany(company);
    this.toast.success(
      this.translation.t('alerts', 'companySelected', { name: company.name }),
    );
  }

  editCompany(company: CompanyResponseDto): void {
    this.companyContext.selectCompany(company);
    void this.router.navigate(['/organization/companies', company.id, 'edit']);
  }

  toggleStatus(company: CompanyResponseDto): void {
    this.pendingCompanyId.set(company.id);
    const request = company.isActive
      ? this.organizationService.deactivateCompany(company.id)
      : this.organizationService.activateCompany(company.id);

    request.pipe(finalize(() => this.pendingCompanyId.set(null))).subscribe({
      next: (updatedCompany) => {
        this.companies.update((companies) =>
          companies.map((item) => (item.id === updatedCompany.id ? updatedCompany : item)),
        );

        if (updatedCompany.isActive) {
          this.companyContext.selectCompany(updatedCompany);
        } else if (this.companyContext.currentCompanyId() === updatedCompany.id) {
          this.companyContext.synchronize(this.companies());
        }

        this.toast.success(
          this.translation.t(
            'alerts',
            updatedCompany.isActive ? 'companyActivated' : 'companyDeactivated',
            { name: updatedCompany.name },
          ),
        );
      },
    });
  }

  private loadCompanies(): void {
    this.isLoading.set(true);
    this.organizationService
      .getCompanies()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (companies) => {
          this.companies.set(companies);
          this.companyContext.synchronize(companies);
        },
      });
  }
}
