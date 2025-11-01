import { BaseApiService } from "./baseApiService";
import type {
  LoginRequestDto,
  RegisterRequestDto,
  LoginResponseDto,
  UserDto,
  ChangePasswordDto,
  User,
  LoginCredentials,
  RegisterCredentials,
  AuthState
} from "@/types";

class AuthService extends BaseApiService {
  private readonly TOKEN_KEY = "auth_token";
  private readonly USER_KEY = "user_data";

  constructor() {
    super();
  }

  // Map UserDto to User model
  private mapUserDtoToUser(dto: UserDto): User {
    return {
      id: dto.id,
      email: dto.email,
      username: dto.username
    };
  }

  // Login user
  async login(credentials: LoginCredentials): Promise<User> {
    try {
      const loginDto: LoginRequestDto = {
        email: credentials.email,
        password: credentials.password
      };

      const response = await this.post<LoginResponseDto>("/auth/login", loginDto);
      
      // Store token and user data
      this.setToken(response.token);
      const user = this.mapUserDtoToUser(response.user);
      this.setUser(user);

      return user;
    } catch (error) {
      console.error("Login failed:", error);
      throw error;
    }
  }

  // Register user
  async register(credentials: RegisterCredentials): Promise<User> {
    try {
      const registerDto: RegisterRequestDto = {
        email: credentials.email,
        password: credentials.password,
        username: credentials.username
      };

      const response = await this.post<LoginResponseDto>("/auth/register", registerDto);
      
      // Store token and user data
      this.setToken(response.token);
      const user = this.mapUserDtoToUser(response.user);
      this.setUser(user);

      return user;
    } catch (error) {
      console.error("Registration failed:", error);
      throw error;
    }
  }

  // Logout user
  async logout(): Promise<void> {
    try {
      // I don't want to implement revoking of JWT, so only local logout

      // Call logout endpoint if token exists
      // if (this.getToken()) {
      //   await this.post("/auth/logout");
      // }
    } catch (error) {
      console.error("Logout request failed:", error);
      // Continue with local logout even if server request fails
    } finally {
      this.clearAuthData();
    }
  }

  // Get current user info
  async getCurrentUser(): Promise<User> {
    try {
      const userDto = await this.get<UserDto>("/auth/me");
      const user = this.mapUserDtoToUser(userDto);
      this.setUser(user);
      return user;
    } catch (error) {
      console.error("Failed to get current user:", error);
      throw error;
    }
  }

  // Change password
  async changePassword(passwordData: ChangePasswordDto): Promise<void> {
    try {
      await this.post("/auth/change-password", passwordData);
    } catch (error) {
      console.error("Password change failed:", error);
      throw error;
    }
  }

  // Token management
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  // User data management
  getUser(): User | null {
    try {
      const userData = localStorage.getItem(this.USER_KEY);
      return userData ? JSON.parse(userData) : null;
    } catch {
      return null;
    }
  }

  setUser(user: User): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  // Check if user is authenticated
  isAuthenticated(): boolean {
    return !!(this.getToken() && this.getUser());
  }

  // Clear all auth data
  clearAuthData(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }

  // Get current auth state
  getAuthState(): AuthState {
    return {
      isAuthenticated: this.isAuthenticated(),
      user: this.getUser(),
      token: this.getToken(),
      loading: false,
      error: null
    };
  }

}

// Export singleton instance
export const authService = new AuthService();
export default authService;