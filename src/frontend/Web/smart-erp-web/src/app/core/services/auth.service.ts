import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  RegisterRequestDto,
  RegisterResponseDto,
} from '../../features/auth/dto/register/register.dto';

import { environment } from '../../../environments/environment';
import {
  ConfirmEmailResponseDto,
  ResendConfirmationEmailRequestDto,
  ResendConfirmationEmailResponseDto,
} from '../../features/auth/dto/email-confirmation/email-confirmation.dto';
import { LoginRequestDto, LoginResponseDto } from '../../features/auth/dto/login/login.dto';
import {
  ForgotPasswordRequestDto,
  ForgotPasswordResponseDto,
  ResetPasswordRequestDto,
  ResetPasswordResponseDto,
} from '../../features/auth/dto/password/password.dto';
import {
  RefreshResponseDto,
  SessionResponseDto,
} from '../../features/auth/dto/session/session.dto';
import { SKIP_API_ERROR_TOAST } from '../interceptors/http-context.tokens';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly httpClient = inject(HttpClient);

  register(requestDto: RegisterRequestDto): Observable<RegisterResponseDto> {
    return this.httpClient.post<RegisterResponseDto>(
      `${environment.identityApiUrl}/register`,
      requestDto,
    );
  }

  confirmEmail(userId: number, token: string): Observable<ConfirmEmailResponseDto> {
    const params = new HttpParams().set('userId', userId).set('token', token);

    return this.httpClient.get<ConfirmEmailResponseDto>(
      `${environment.identityApiUrl}/confirm-email`,
      { params },
    );
  }

  resendConfirmationEmail(
    requestDto: ResendConfirmationEmailRequestDto,
  ): Observable<ResendConfirmationEmailResponseDto> {
    return this.httpClient.post<ResendConfirmationEmailResponseDto>(
      `${environment.identityApiUrl}/resend-confirmation-email`,
      requestDto,
    );
  }
  login(requestDto: LoginRequestDto): Observable<LoginResponseDto> {
    return this.httpClient.post<LoginResponseDto>(
      `${environment.identityApiUrl}/login`,
      requestDto,
      {
        withCredentials: true,
      },
    );
  }

  forgotPassword(
    requestDto: ForgotPasswordRequestDto,
  ): Observable<ForgotPasswordResponseDto> {
    return this.httpClient.post<ForgotPasswordResponseDto>(
      `${environment.identityApiUrl}/forgot-password`,
      requestDto,
    );
  }

  resetPassword(
    requestDto: ResetPasswordRequestDto,
  ): Observable<ResetPasswordResponseDto> {
    return this.httpClient.post<ResetPasswordResponseDto>(
      `${environment.identityApiUrl}/reset-password`,
      requestDto,
    );
  }

  refresh(silent = false): Observable<RefreshResponseDto> {
    const context = silent
      ? new HttpContext().set(SKIP_API_ERROR_TOAST, true)
      : new HttpContext();

    return this.httpClient.post<RefreshResponseDto>(
      `${environment.identityApiUrl}/refresh`,
      {},
      { withCredentials: true, context },
    );
  }

  getSessions(): Observable<SessionResponseDto[]> {
    return this.httpClient.get<SessionResponseDto[]>(
      `${environment.identityApiUrl}/sessions`,
      { withCredentials: true },
    );
  }

  revokeSession(sessionId: number): Observable<void> {
    return this.httpClient.delete<void>(
      `${environment.identityApiUrl}/sessions/${sessionId}`,
      { withCredentials: true },
    );
  }

  logout(): Observable<void> {
    return this.httpClient.post<void>(
      `${environment.identityApiUrl}/logout`,
      {},
      { withCredentials: true },
    );
  }

  logoutAll(): Observable<void> {
    return this.httpClient.post<void>(
      `${environment.identityApiUrl}/logout-all`,
      {},
      { withCredentials: true },
    );
  }

  startGoogleLogin(): void {
    const frontendOrigin = globalThis.location.origin;
    globalThis.location.assign(
      `${environment.identityApiUrl}/google?frontendOrigin=${encodeURIComponent(frontendOrigin)}`,
    );
  }
}
