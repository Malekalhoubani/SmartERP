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
    path: '',
    pathMatch: 'full',
    redirectTo: 'auth/register',
  },
  {
    path: '**',
    redirectTo: 'auth/register',
  },
];
