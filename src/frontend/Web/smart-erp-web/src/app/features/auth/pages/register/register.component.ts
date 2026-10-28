import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { finalize } from 'rxjs';
import { TranslationService } from '../../../../core/i18n/translation.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { RegisterRequestDto } from '../../dto/register/register.dto';
import { Router, RouterLink } from '@angular/router';
const passwordsMatchValidator = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordsMismatch: true };
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly translation = inject(TranslationService);
  readonly isSubmitting = signal(false);
  readonly currentYear = new Date().getFullYear();
  private readonly router = inject(Router);

  readonly registerForm = this.formBuilder.nonNullable.group(
    {
      fullName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(12),
          Validators.pattern(/[A-Z]/),
          Validators.pattern(/[a-z]/),
          Validators.pattern(/[0-9]/),
          Validators.pattern(/[^a-zA-Z0-9]/),
        ],
      ],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: passwordsMatchValidator },
  );

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const requestDto: RegisterRequestDto = this.registerForm.getRawValue();
    this.isSubmitting.set(true);

    this.authService
      .register(requestDto)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (responseDto) => {
          this.toast.success(
            this.translation.t('alerts', 'registerSuccess', { email: responseDto.email }),
          );

          this.registerForm.reset();

          void this.router.navigate(['/auth/check-email'], {
            queryParams: {
              email: responseDto.email,
            },
            replaceUrl: true,
          });
        },
        error: () => {
          // The global API interceptor displays a localized error toast.
        },
      });
  }
}
