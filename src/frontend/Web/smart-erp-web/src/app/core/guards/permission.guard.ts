import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthStateService } from '../services/auth-state.service';

export const permissionGuard: CanActivateFn = (route) => {
  const authState = inject(AuthStateService);
  const router = inject(Router);
  const permission = route.data['permission'] as string | undefined;

  return !permission || authState.hasPermission(permission)
    ? true
    : router.createUrlTree(['/account/sessions']);
};
