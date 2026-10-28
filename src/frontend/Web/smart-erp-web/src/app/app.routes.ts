import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

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
    path: 'account/sessions',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/auth/pages/sessions/sessions.component').then(
        (component) => component.SessionsComponent,
      ),
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'account/sessions',
  },
  {
    path: '**',
    redirectTo: 'auth/register',
  },
];
