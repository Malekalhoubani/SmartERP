import { HttpBackend, HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { finalize, map, Observable, shareReplay, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { RefreshResponseDto } from '../../features/auth/dto/session/session.dto';
import { AuthStateService } from './auth-state.service';

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly rawHttpClient = new HttpClient(inject(HttpBackend));
  private readonly authState = inject(AuthStateService);
  private refreshRequest: Observable<string> | null = null;

  refreshAccessToken(): Observable<string> {
    if (!this.refreshRequest) {
      this.refreshRequest = this.rawHttpClient
        .post<RefreshResponseDto>(
          `${environment.identityApiUrl}/refresh`,
          {},
          { withCredentials: true },
        )
        .pipe(
          tap((response) =>
            this.authState.setAccessToken(
              response.accessToken,
              response.accessTokenExpiresAtUtc,
            ),
          ),
          map((response) => response.accessToken),
          finalize(() => (this.refreshRequest = null)),
          shareReplay({ bufferSize: 1, refCount: false }),
        );
    }

    return this.refreshRequest;
  }
}
