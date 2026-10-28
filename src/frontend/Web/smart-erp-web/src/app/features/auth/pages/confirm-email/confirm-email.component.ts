import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';

type ConfirmationStatus = 'loading' | 'success' | 'error';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.scss',
})
export class ConfirmEmailComponent implements OnInit {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly authService = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly translation = inject(TranslationService);
  readonly status = signal<ConfirmationStatus>('loading');

  ngOnInit(): void {
    const userIdValue = this.activatedRoute.snapshot.queryParamMap.get('userId');
    const token = this.activatedRoute.snapshot.queryParamMap.get('token')?.trim() ?? '';
    const userId = Number(userIdValue);

    if (!Number.isInteger(userId) || userId <= 0 || !token) {
      this.status.set('error');
      this.toast.error(
        this.translation.t('alerts', 'invalidConfirmationLink'),
      );
      return;
    }

    this.authService.confirmEmail(userId, token).subscribe({
      next: () => {
        this.status.set('success');
        this.toast.success(
          this.translation.t('alerts', 'emailConfirmedSuccess'),
        );
      },
      error: () => {
        this.status.set('error');
        // The global API interceptor displays the localized server error.
      },
    });
  }
}
