export interface ForgotPasswordRequestDto {
  email: string;
}

export interface ForgotPasswordResponseDto {
  message: string;
}

export interface ResetPasswordRequestDto {
  userId: number;
  token: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ResetPasswordResponseDto {
  message: string;
}
