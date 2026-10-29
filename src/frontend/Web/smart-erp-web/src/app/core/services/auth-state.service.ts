import { computed, Injectable, signal } from '@angular/core';
import { LoginResponseDto } from '../../features/auth/dto/login/login.dto';

export interface AuthenticatedUser {
  userId: number;
  fullName: string;
  email: string;
}

interface JwtPayload {
  sub?: string;
  name?: string;
  email?: string;
  role?: string | string[];
  permission?: string | string[];
  sid?: string;
  exp?: number;
}
@Injectable({
  providedIn: 'root',
})
export class AuthStateService {
  private readonly accessTokenSignal = signal<string | null>(null);
  private readonly accessTokenExpiresAtSignal = signal<string | null>(null);
  private readonly currentUserSignal = signal<AuthenticatedUser | null>(null);
  private readonly rolesSignal = signal<readonly string[]>([]);
  private readonly permissionsSignal = signal<readonly string[]>([]);

  readonly accessToken = this.accessTokenSignal.asReadonly();
  readonly accessTokenExpiresAt = this.accessTokenExpiresAtSignal.asReadonly();
  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly roles = this.rolesSignal.asReadonly();
  readonly permissions = this.permissionsSignal.asReadonly();

  readonly isAuthenticated = computed(() => this.accessTokenSignal() !== null);

  setSession(responseDto: LoginResponseDto): void {
    this.setAccessToken(
      responseDto.accessToken,
      responseDto.accessTokenExpiresAtUtc,
    );

    if (!this.currentUserSignal()) {
      this.currentUserSignal.set({
        userId: responseDto.userId,
        fullName: responseDto.fullName,
        email: responseDto.email,
      });
    }
  }

  setAccessToken(accessToken: string, expiresAtUtc?: string): boolean {
    const payload = this.decodePayload(accessToken);
    const userId = Number(payload?.sub);

    if (!payload || !Number.isInteger(userId) || userId <= 0) {
      this.clearSession();
      return false;
    }

    this.accessTokenSignal.set(accessToken);
    this.accessTokenExpiresAtSignal.set(
      expiresAtUtc ??
        (payload.exp ? new Date(payload.exp * 1000).toISOString() : null),
    );
    this.currentUserSignal.set({
      userId,
      fullName: payload.name ?? '',
      email: payload.email ?? '',
    });
    this.rolesSignal.set(this.toStringArray(payload.role));
    this.permissionsSignal.set(this.toStringArray(payload.permission));
    return true;
  }

  getAccessToken(): string | null {
    return this.accessTokenSignal();
  }

  isAccessTokenExpired(leewaySeconds = 30): boolean {
    const expiresAt = this.accessTokenExpiresAtSignal();
    if (!expiresAt) return true;

    return new Date(expiresAt).getTime() <=
      Date.now() + leewaySeconds * 1000;
  }

  hasRole(role: string): boolean {
    return this.rolesSignal().includes(role);
  }

  hasPermission(permission: string): boolean {
    return this.permissionsSignal().includes(permission);
  }

  clearSession(): void {
    this.accessTokenSignal.set(null);
    this.accessTokenExpiresAtSignal.set(null);
    this.currentUserSignal.set(null);
    this.rolesSignal.set([]);
    this.permissionsSignal.set([]);
  }

  private decodePayload(accessToken: string): JwtPayload | null {
    try {
      const encodedPayload = accessToken.split('.')[1];
      if (!encodedPayload) return null;

      const normalized = encodedPayload
        .replace(/-/g, '+')
        .replace(/_/g, '/')
        .padEnd(Math.ceil(encodedPayload.length / 4) * 4, '=');
      const bytes = Uint8Array.from(atob(normalized), (character) =>
        character.charCodeAt(0),
      );

      return JSON.parse(new TextDecoder().decode(bytes)) as JwtPayload;
    } catch {
      return null;
    }
  }

  private toStringArray(value: string | string[] | undefined): string[] {
    if (Array.isArray(value)) return value;
    return value ? [value] : [];
  }
}
