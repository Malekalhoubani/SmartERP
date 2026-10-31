import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { TranslationService } from '../../../../core/i18n/translation.service';
import { AuthService } from '../../../../core/services/auth.service';
import { AuthStateService } from '../../../../core/services/auth-state.service';
import { ToastService } from '../../../../core/services/toast.service';
import { LoginRequestDto } from '../../dto/login/login.dto';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly authState = inject(AuthStateService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly translation = inject(TranslationService);
  readonly isSubmitting = signal(false);
  readonly showPassword = signal(false);
  readonly currentYear = new Date().getFullYear();

  readonly loginForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    password: ['', [Validators.required, Validators.maxLength(128)]],
  });

  togglePasswordVisibility(): void {
    this.showPassword.update((isVisible) => !isVisible);
  }

  loginWithGoogle(): void {
    this.authService.startGoogleLogin();
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const requestDto: LoginRequestDto = this.loginForm.getRawValue();

    this.isSubmitting.set(true);

    this.authService
      .login(requestDto)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (responseDto) => {
          this.authState.setSession(responseDto);

          this.toast.success(
            this.translation.t('alerts', 'loginSuccess', {
              name: responseDto.fullName,
            }),
          );

          this.loginForm.reset();
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
          void this.router.navigateByUrl(returnUrl || '/organization/companies');
        },
        error: () => {
        },
      });
  }
}
