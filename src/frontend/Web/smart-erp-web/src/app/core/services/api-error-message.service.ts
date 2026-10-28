import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { TranslationService } from '../i18n/translation.service';

@Injectable({ providedIn: 'root' })
export class ApiErrorMessageService {
  private readonly translation = inject(TranslationService);

  getMessage(error: HttpErrorResponse): string {
    return this.translation.t('alerts', this.getMessageKey(error.status));
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
