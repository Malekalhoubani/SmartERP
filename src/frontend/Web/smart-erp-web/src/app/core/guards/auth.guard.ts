import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';

import { AuthSessionService } from '../services/auth-session.service';
import { AuthStateService } from '../services/auth-state.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const authState = inject(AuthStateService);
  const authSession = inject(AuthSessionService);
  const router = inject(Router);

  if (authState.isAuthenticated() && !authState.isAccessTokenExpired()) {
    return true;
  }

  return authSession.refreshAccessToken().pipe(
    map(() => true),
    catchError(() => {
      authState.clearSession();
      return of(
        router.createUrlTree(['/auth/login'], {
          queryParams: { returnUrl: state.url },
        }),
      );
    }),
  );
};
