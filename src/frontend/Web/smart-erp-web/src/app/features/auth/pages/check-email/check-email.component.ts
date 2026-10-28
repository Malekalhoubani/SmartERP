import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { finalize, map } from 'rxjs';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-check-email',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './check-email.component.html',
  styleUrl: './check-email.component.scss',
})
export class CheckEmailComponent {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly authService = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly translation = inject(TranslationService);
  readonly isResending = signal(false);

  readonly email = toSignal(
    this.activatedRoute.queryParamMap.pipe(
      map((queryParameters) => queryParameters.get('email')?.trim() ?? ''),
    ),
    {
      initialValue: '',
    },
  );

  resendConfirmationEmail(): void {
    const email = this.email();

    if (!email) {
      this.toast.warning(
        this.translation.t('alerts', 'emailRequiredForResend'),
      );
      return;
    }

    this.isResending.set(true);

    this.authService
      .resendConfirmationEmail({ email })
      .pipe(finalize(() => this.isResending.set(false)))
      .subscribe({
        next: () => {
          this.toast.success(
            this.translation.t(
              'alerts',
              'confirmationEmailResent',
              { email },
            ),
          );
        },
        error: () => {
          // The global API interceptor displays a localized error toast.
        },
      });
  }
}
