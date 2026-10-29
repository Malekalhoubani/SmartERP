export interface RefreshResponseDto {
  accessToken: string;
  accessTokenExpiresAtUtc: string;
}

export interface SessionResponseDto {
  sessionId: number;
  ipAddress: string | null;
  userAgent: string | null;
  createdAtUtc: string;
  lastActivityAtUtc: string;
  expiresAtUtc: string;
  isCurrent: boolean;
}
