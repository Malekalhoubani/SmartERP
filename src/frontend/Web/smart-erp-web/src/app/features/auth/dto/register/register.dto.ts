export interface RegisterRequestDto {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterResponseDto {
  userId: number;
  email: string;
  requiresEmailConfirmation: boolean;
}