import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { companyAccessGuard } from './core/guards/company-access.guard';
import { currentCompanyGuard } from './core/guards/company-access.guard';

export const routes: Routes = [
  {
    path: 'auth/register',
    loadComponent: () =>
      import('./features/auth/pages/register/register.component').then(
        (component) => component.RegisterComponent,
      ),
  },
  {
    path: 'auth/check-email',
    loadComponent: () =>
      import('./features/auth/pages/check-email/check-email.component').then(
        (component) => component.CheckEmailComponent,
      ),
  },
  {
    path: 'auth/confirm-email',
    loadComponent: () =>
      import('./features/auth/pages/confirm-email/confirm-email.component').then(
        (component) => component.ConfirmEmailComponent,
      ),
  },
  {
    path: 'auth/login',
    loadComponent: () =>
      import('./features/auth/pages/login/login.component').then(
        (component) => component.LoginComponent,
      ),
  },
  {
    path: 'auth/forgot-password',
    loadComponent: () =>
      import('./features/auth/pages/forgot-password/forgot-password.component').then(
        (component) => component.ForgotPasswordComponent,
      ),
  },
  {
    path: 'auth/reset-password',
    loadComponent: () =>
      import('./features/auth/pages/reset-password/reset-password.component').then(
        (component) => component.ResetPasswordComponent,
      ),
  },
  {
    path: 'auth/google-callback',
    loadComponent: () =>
      import('./features/auth/pages/google-callback/google-callback.component').then(
        (component) => component.GoogleCallbackComponent,
      ),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./shared/components/app-shell/app-shell.component').then(
        (component) => component.AppShellComponent,
      ),
    children: [
      {
        path: 'account/sessions',
        loadComponent: () => import('./features/auth/pages/sessions/sessions.component').then((component) => component.SessionsComponent),
      },
      {
        path: 'organization/companies',
        loadComponent: () => import('./features/organization/pages/companies/companies.component').then((component) => component.CompaniesComponent),
      },
      {
        path: 'organization/companies/new',
        loadComponent: () => import('./features/organization/pages/company-form/company-form.component').then((component) => component.CompanyFormComponent),
      },
      {
        path: 'organization/companies/:companyId/edit',
        canActivate: [companyAccessGuard],
        loadComponent: () => import('./features/organization/pages/company-form/company-form.component').then((component) => component.CompanyFormComponent),
      },
      {
        path: 'catalog/:type',
        canActivate: [currentCompanyGuard],
        loadComponent: () => import('./features/catalog/pages/catalog-list/catalog-list.component').then((component) => component.CatalogListComponent),
      },
      { path: 'organization', pathMatch: 'full', redirectTo: 'organization/companies' },
      { path: '', pathMatch: 'full', redirectTo: 'organization/companies' },
    ],
  },
  {
    path: '**',
    redirectTo: 'auth/login',
  },
];
