import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
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
}
