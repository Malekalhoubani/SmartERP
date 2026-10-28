import { Routes } from '@angular/router';

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
    path: '',
    pathMatch: 'full',
    redirectTo: 'auth/register',
  },
  {
    path: '**',
    redirectTo: 'auth/register',
  },
];
