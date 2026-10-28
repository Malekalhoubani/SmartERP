import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ApiErrorMessageService } from '../services/api-error-message.service';
import { ToastService } from '../services/toast.service';
import { SKIP_API_ERROR_TOAST } from './http-context.tokens';

export const apiErrorInterceptor: HttpInterceptorFn = (request, next) => {
  const errorMessages = inject(ApiErrorMessageService);
  const toast = inject(ToastService);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (!request.context.get(SKIP_API_ERROR_TOAST)) {
        toast.error(errorMessages.getMessage(error));
      }
      return throwError(() => error);
    }),
  );
};
