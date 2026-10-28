export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface LoginResponseDto {
  userId: number;
  fullName: string;
  email: string;
  accessToken: string;
  accessTokenExpiresAtUtc: string;
}
