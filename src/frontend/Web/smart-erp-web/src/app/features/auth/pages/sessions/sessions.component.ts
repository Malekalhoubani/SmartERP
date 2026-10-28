import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { AuthStateService } from '../../../../core/services/auth-state.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { SessionResponseDto } from '../../dto/session/session.dto';

@Component({
  selector: 'app-sessions',
  standalone: true,
  imports: [DatePipe, HasPermissionDirective],
  templateUrl: './sessions.component.html',
  styleUrl: './sessions.component.scss',
})
export class SessionsComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly authState = inject(AuthStateService);
  readonly translation = inject(TranslationService);
  readonly sessions = signal<SessionResponseDto[]>([]);
  readonly isLoading = signal(true);
  readonly pendingSessionId = signal<number | null>(null);
  readonly isLoggingOut = signal(false);

  ngOnInit(): void {
    this.loadSessions();
  }

  loadSessions(): void {
    this.isLoading.set(true);
    this.authService
      .getSessions()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (sessions) => this.sessions.set(sessions),
        error: () => undefined,
      });
  }

  revokeSession(session: SessionResponseDto): void {
    this.pendingSessionId.set(session.sessionId);
    this.authService
      .revokeSession(session.sessionId)
      .pipe(finalize(() => this.pendingSessionId.set(null)))
      .subscribe({
        next: () => {
          this.toast.success(this.translation.t('alerts', 'sessionRevoked'));
          if (session.isCurrent) {
            this.finishLogout();
          } else {
            this.loadSessions();
          }
        },
        error: () => undefined,
      });
  }

  logout(): void {
    this.isLoggingOut.set(true);
    this.authService
      .logout()
      .pipe(finalize(() => this.isLoggingOut.set(false)))
      .subscribe({ next: () => this.finishLogout(), error: () => undefined });
  }

  logoutAll(): void {
    this.isLoggingOut.set(true);
    this.authService
      .logoutAll()
      .pipe(finalize(() => this.isLoggingOut.set(false)))
      .subscribe({ next: () => this.finishLogout(), error: () => undefined });
  }

  private finishLogout(): void {
    this.authState.clearSession();
    this.toast.success(this.translation.t('alerts', 'logoutSuccess'));
    void this.router.navigate(['/auth/login'], { replaceUrl: true });
  }
}
