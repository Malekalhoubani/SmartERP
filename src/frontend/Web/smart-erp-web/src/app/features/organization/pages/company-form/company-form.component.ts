import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { CompanyContextService } from '../../../../core/services/company-context.service';
import { OrganizationService } from '../../../../core/services/organization.service';
import { ToastService } from '../../../../core/services/toast.service';
import {
  CreateCompanyRequestDto,
  UpdateCompanyRequestDto,
} from '../../dto/company/company.dto';

@Component({
  selector: 'app-company-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './company-form.component.html',
  styleUrl: './company-form.component.scss',
})
export class CompanyFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly organizationService = inject(OrganizationService);
  private readonly companyContext = inject(CompanyContextService);
  private readonly toast = inject(ToastService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly translation = inject(TranslationService);
  readonly isSubmitting = signal(false);
  readonly isLoading = signal(false);
  readonly companyId = signal<number | null>(null);
  readonly isEditMode = signal(false);

  readonly companyForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    legalName: ['', [Validators.required, Validators.maxLength(200)]],
    code: [
      '',
      [Validators.required, Validators.maxLength(30), Validators.pattern(/^[A-Za-z0-9_-]+$/)],
    ],
    registrationNumber: ['', Validators.maxLength(100)],
    taxNumber: ['', Validators.maxLength(100)],
    email: ['', [Validators.email, Validators.maxLength(256)]],
    phone: ['', Validators.maxLength(30)],
  });

  ngOnInit(): void {
    const companyId = Number(this.route.snapshot.paramMap.get('companyId'));
    if (Number.isInteger(companyId) && companyId > 0) {
      this.companyId.set(companyId);
      this.isEditMode.set(true);
      this.companyForm.controls.code.disable();
      this.loadCompany(companyId);
    }
  }

  hasError(controlName: keyof typeof this.companyForm.controls, errorName: string): boolean {
    const control = this.companyForm.controls[controlName];
    return control.touched && control.hasError(errorName);
  }

  onSubmit(): void {
    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const value = this.companyForm.getRawValue();
    const sharedRequest: UpdateCompanyRequestDto = {
      name: value.name.trim(),
      legalName: value.legalName.trim(),
      registrationNumber: this.optional(value.registrationNumber),
      taxNumber: this.optional(value.taxNumber),
      email: this.optional(value.email),
      phone: this.optional(value.phone),
    };

    const companyId = this.companyId();
    const request = companyId
      ? this.organizationService.updateCompany(companyId, sharedRequest)
      : this.organizationService.createCompany({
          ...sharedRequest,
          code: value.code.trim().toUpperCase(),
        } satisfies CreateCompanyRequestDto);

    request.pipe(finalize(() => this.isSubmitting.set(false))).subscribe({
      next: (company) => {
        this.companyContext.selectCompany(company);
        this.toast.success(
          this.translation.t(
            'alerts',
            companyId ? 'companyUpdated' : 'companyCreated',
            { name: company.name },
          ),
        );
        void this.router.navigate(['/organization/companies']);
      },
    });
  }

  private loadCompany(companyId: number): void {
    this.isLoading.set(true);
    this.organizationService
      .getCompany(companyId)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (company) => {
          this.companyForm.patchValue({
            name: company.name,
            legalName: company.legalName,
            code: company.code,
            registrationNumber: company.registrationNumber ?? '',
            taxNumber: company.taxNumber ?? '',
            email: company.email ?? '',
            phone: company.phone ?? '',
          });
        },
      });
  }

  private optional(value: string): string | null {
    const normalized = value.trim();
    return normalized.length > 0 ? normalized : null;
  }
}
