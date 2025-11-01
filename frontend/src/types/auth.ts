// Authentication related DTOs matching backend

export interface LoginRequestDto {
  email: string
  password: string
}

export interface RegisterRequestDto {
  email: string
  password: string
  username: string
}

export interface LoginResponseDto {
  token: string
  user: UserDto
}

export interface UserDto {
  id: number
  email: string
  username: string
}

export interface ChangePasswordDto {
  currentPassword: string
  newPassword: string
  confirmPassword: string
}

export interface ForgotPasswordDto {
  email: string
}

export interface ResetPasswordDto {
  token: string
  password: string
  confirmPassword: string
}