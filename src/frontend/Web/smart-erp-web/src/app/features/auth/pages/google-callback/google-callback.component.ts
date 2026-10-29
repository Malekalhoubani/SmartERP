import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { AuthStateService } from '../../../../core/services/auth-state.service';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-google-callback',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './google-callback.component.html',
  styleUrl: '../auth-action-page.scss',
})
export class GoogleCallbackComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authState = inject(AuthStateService);
  private readonly toast = inject(ToastService);

  readonly translation = inject(TranslationService);
  readonly failed = signal(false);

  ngOnInit(): void {
    const fragment = new URLSearchParams(this.route.snapshot.fragment ?? '');
    const accessToken = fragment.get('accessToken');
    const expiresAtUtc = fragment.get('expiresAtUtc') ?? undefined;

    if (!accessToken || !this.authState.setAccessToken(accessToken, expiresAtUtc)) {
      this.failed.set(true);
      this.toast.error(this.translation.t('alerts', 'googleLoginFailed'));
      return;
    }

    this.toast.success(this.translation.t('alerts', 'googleLoginSuccess'));
    void this.router.navigate(['/account/sessions'], { replaceUrl: true });
  }
}
