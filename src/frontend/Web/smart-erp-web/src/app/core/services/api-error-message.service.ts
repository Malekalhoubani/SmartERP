import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { TranslationService } from '../i18n/translation.service';

@Injectable({ providedIn: 'root' })
export class ApiErrorMessageService {
  private readonly translation = inject(TranslationService);

  getMessage(error: HttpErrorResponse): string {
    const errorCode = this.getErrorCode(error);

    if (errorCode) {
      return this.translation.t('alerts', this.getErrorCodeMessageKey(errorCode));
    }

    return this.translation.t('alerts', this.getMessageKey(error.status));
  }

  private getErrorCode(error: HttpErrorResponse): string | null {
    const code = error.error?.code;
    return typeof code === 'string' && code.length > 0 ? code : null;
  }

  private getErrorCodeMessageKey(code: string): string {
    const messageKeys: Record<string, string> = {
      invalid_credentials: 'invalidCredentials',
      email_not_confirmed: 'emailConfirmationRequired',
      account_locked: 'accountLocked',
    };

    return messageKeys[code] ?? 'requestFailed';
  }

  private getMessageKey(status: number): string {
    if (status === 0) return 'apiOffline';
    if (status === 400) return 'badRequest';
    if (status === 401) return 'unauthorized';
    if (status === 403) return 'forbidden';
    if (status === 404) return 'notFound';
    if (status === 409) return 'conflict';
    if (status === 429) return 'tooManyRequests';
    if (status >= 500) return 'serverError';
    return 'requestFailed';
  }
}
