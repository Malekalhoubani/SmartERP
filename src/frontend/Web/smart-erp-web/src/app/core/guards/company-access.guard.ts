import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';

import { CompanyContextService } from '../services/company-context.service';
import { OrganizationService } from '../services/organization.service';

export const companyAccessGuard: CanActivateFn = (route) => {
  const companyId = Number(route.paramMap.get('companyId'));
  const organizationService = inject(OrganizationService);
  const companyContext = inject(CompanyContextService);
  const router = inject(Router);

  if (!Number.isInteger(companyId) || companyId <= 0) {
    return router.createUrlTree(['/organization/companies']);
  }

  return organizationService.getCompany(companyId).pipe(
    map((company) => {
      companyContext.selectCompany(company);
      return true;
    }),
    catchError(() => of(router.createUrlTree(['/organization/companies']))),
  );
};

export const currentCompanyGuard: CanActivateFn = () => {
  const companyContext = inject(CompanyContextService);
  const router = inject(Router);

  return companyContext.hasCurrentCompany()
    ? true
    : router.createUrlTree(['/organization/companies']);
};
