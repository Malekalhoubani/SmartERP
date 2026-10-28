import { computed, Injectable, signal } from '@angular/core';
import { LoginResponseDto } from '../../features/auth/dto/login/login.dto';

export interface AuthenticatedUser {
  userId: number;
  fullName: string;
  email: string;
}
@Injectable({
  providedIn: 'root',
})
export class AuthStateService {
  private readonly accessTokenSignal = signal<string | null>(null);
  private readonly accessTokenExpiresAtSignal = signal<string | null>(null);
  private readonly currentUserSignal = signal<AuthenticatedUser | null>(null);

  readonly accessToken = this.accessTokenSignal.asReadonly();
  readonly accessTokenExpiresAt = this.accessTokenExpiresAtSignal.asReadonly();
  readonly currentUser = this.currentUserSignal.asReadonly();

  readonly isAuthenticated = computed(() => this.accessTokenSignal() !== null);

  setSession(responseDto: LoginResponseDto): void {
    this.accessTokenSignal.set(responseDto.accessToken);
    this.accessTokenExpiresAtSignal.set(responseDto.accessTokenExpiresAtUtc);

    this.currentUserSignal.set({
      userId: responseDto.userId,
      fullName: responseDto.fullName,
      email: responseDto.email,
    });
  }

  getAccessToken(): string | null {
    return this.accessTokenSignal();
  }

  clearSession(): void {
    this.accessTokenSignal.set(null);
    this.accessTokenExpiresAtSignal.set(null);
    this.currentUserSignal.set(null);
  }
}
