import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  RegisterRequestDto,
  RegisterResponseDto,
} from '../../features/auth/dto/register/register.dto';

import { environment } from '../../../environments/environment';

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
}
