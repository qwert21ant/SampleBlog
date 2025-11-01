import { ref, computed, reactive, readonly } from "vue";
import { useRouter } from "vue-router";
import { authService } from "@/services/authService";
import type { 
  AuthState, 
  User, 
  LoginCredentials, 
  RegisterCredentials,
  ValidationErrors 
} from "@/types";

// Global auth state
const authState = reactive<AuthState>({
  isAuthenticated: false,
  user: null,
  token: null,
  loading: false,
  error: null
});

// Initialize auth state from localStorage
const initializeAuth = () => {
  const state = authService.getAuthState();
  Object.assign(authState, state);
};

export function useAuth() {
  const router = useRouter();
  
  // Computed properties
  const isAuthenticated = computed(() => authState.isAuthenticated);
  const user = computed(() => authState.user);
  const isLoading = computed(() => authState.loading);
  const error = computed(() => authState.error);

  // Validation
  const validateLoginCredentials = (credentials: LoginCredentials): ValidationErrors => {
    const errors: ValidationErrors = {};

    if (!credentials.email) {
      errors.email = "Email is required";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(credentials.email)) {
      errors.email = "Please enter a valid email address";
    }

    if (!credentials.password) {
      errors.password = "Password is required";
    } else if (credentials.password.length < 6) {
      errors.password = "Password must be at least 6 characters";
    }

    return errors;
  };

  const validateRegisterCredentials = (credentials: RegisterCredentials): ValidationErrors => {
    const errors = validateLoginCredentials(credentials);

    if (credentials.password !== credentials.confirmPassword) {
      errors.confirmPassword = "Passwords do not match";
    }

    if (!credentials.username) {
      errors.username = "Username is required";
    } else if (credentials.username.length < 3) {
      errors.username = "Username must be at least 3 characters";
    } else if (credentials.username.length > 30) {
      errors.username = "Username must be less than 30 characters";
    } else if (!/^[a-zA-Z0-9_-]+$/.test(credentials.username)) {
      errors.username = "Username can only contain letters, numbers, underscores, and hyphens";
    }

    return errors;
  };

  // Actions
  const login = async (credentials: LoginCredentials): Promise<boolean> => {
    try {
      authState.loading = true;
      authState.error = null;

      // Validate credentials
      const validationErrors = validateLoginCredentials(credentials);
      if (Object.keys(validationErrors).length > 0) {
        authState.error = Object.values(validationErrors)[0] || "Validation failed";
        return false;
      }

      const user = await authService.login(credentials);
      
      // Update auth state
      authState.isAuthenticated = true;
      authState.user = user;
      authState.token = authService.getToken();

      return true;
    } catch (error: any) {
      console.error("Login error:", error);
      authState.error = error?.message || "Login failed. Please check your credentials.";
      return false;
    } finally {
      authState.loading = false;
    }
  };

  const register = async (credentials: RegisterCredentials): Promise<boolean> => {
    try {
      authState.loading = true;
      authState.error = null;

      // Validate credentials
      const validationErrors = validateRegisterCredentials(credentials);
      if (Object.keys(validationErrors).length > 0) {
        authState.error = Object.values(validationErrors)[0] || "Validation failed";
        return false;
      }

      const user = await authService.register(credentials);
      
      // Update auth state
      authState.isAuthenticated = true;
      authState.user = user;
      authState.token = authService.getToken();

      return true;
    } catch (error: any) {
      console.error("Registration error:", error);
      authState.error = error?.message || "Registration failed. Please try again.";
      return false;
    } finally {
      authState.loading = false;
    }
  };

  const logout = async (): Promise<void> => {
    try {
      authState.loading = true;
      await authService.logout();
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      // Always clear auth state
      authState.isAuthenticated = false;
      authState.user = null;
      authState.token = null;
      authState.loading = false;
      authState.error = null;
      
      // Redirect to home
      router.push("/");
    }
  };

  const refreshUserData = async (): Promise<void> => {
    try {
      if (!authState.isAuthenticated) return;

      const user = await authService.getCurrentUser();
      authState.user = user;
    } catch (error) {
      console.error("Failed to refresh user data:", error);
      // Don't logout on failure, just log the error
    }
  };

  const clearError = (): void => {
    authState.error = null;
  };

  const requireAuth = (): boolean => {
    if (!isAuthenticated.value) {
      router.push("/");
      return false;
    }
    return true;
  };

  const requireAdmin = (): boolean => {
    if (!isAuthenticated.value) {
      router.push("/");
      return false;
    }
    return true;
  };

  // Initialize auth state if not done
  if (!authState.isAuthenticated && authService.isAuthenticated()) {
    initializeAuth();
  }

  return {
    // State
    isAuthenticated,
    user,
    isLoading,
    error,
    authState: readonly(authState),

    // Actions
    login,
    register,
    logout,
    refreshUserData,
    clearError,
    requireAuth,
    requireAdmin,

    // Validation
    validateLoginCredentials,
    validateRegisterCredentials,

    // Utils
    initializeAuth
  };
}

// Export for external initialization
export { initializeAuth };