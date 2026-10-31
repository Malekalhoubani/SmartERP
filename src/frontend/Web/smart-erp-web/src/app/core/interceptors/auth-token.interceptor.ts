import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthSessionService } from '../services/auth-session.service';
import { AuthStateService } from '../services/auth-state.service';
import { AUTH_RETRY } from './http-context.tokens';

const publicAuthenticationPaths = [
  '/register',
  '/login',
  '/forgot-password',
  '/reset-password',
  '/confirm-email',
  '/resend-confirmation-email',
  '/google',
  '/refresh',
];

export const authTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const authState = inject(AuthStateService);
  const authSession = inject(AuthSessionService);
  const router = inject(Router);
  const isGatewayRequest = request.url.startsWith(environment.gatewayApiUrl);
  const isIdentityRequest = request.url.startsWith(environment.identityApiUrl);
  const isPublicRequest =
    isIdentityRequest &&
    publicAuthenticationPaths.some((path) => request.url.includes(path));

  let outgoingRequest = isIdentityRequest
    ? request.clone({ withCredentials: true })
    : request;
  const accessToken = authState.getAccessToken();

  if (accessToken && isGatewayRequest && !isPublicRequest) {
    outgoingRequest = outgoingRequest.clone({
      setHeaders: { Authorization: `Bearer ${accessToken}` },
    });
  }

  return next(outgoingRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (
        error.status !== 401 ||
        !isGatewayRequest ||
        isPublicRequest ||
        request.context.get(AUTH_RETRY)
      ) {
        return throwError(() => error);
      }

      return authSession.refreshAccessToken().pipe(
        switchMap((newAccessToken) =>
          next(
            outgoingRequest.clone({
              context: outgoingRequest.context.set(AUTH_RETRY, true),
              setHeaders: {
                Authorization: `Bearer ${newAccessToken}`,
              },
            }),
          ),
        ),
        catchError((refreshError: unknown) => {
          authState.clearSession();
          void router.navigate(['/auth/login']);
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
