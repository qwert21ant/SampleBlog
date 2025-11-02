// Centralized type exports

// DTOs (Data Transfer Objects from backend)
export type {
  PostSummaryDto,
  PostDto,
  PostPublicDto,
  CreatePostDto,
  UpdatePostDto,
  PaginatedResult,
  PostStatsDto,
  ImageDetailsDto
} from "./dtos";

// Authentication DTOs
export type {
  LoginRequestDto,
  RegisterRequestDto,
  LoginResponseDto,
  UserDto,
  ChangePasswordDto,
  ForgotPasswordDto,
  ResetPasswordDto
} from "./auth";

// Frontend Models
export type {
  PublicPost,
  PostCard,
  AdminPost,
  PostFilters,
  PostSortOptions,
  PaginationOptions,
  PostSearchResult,
  BlogStats,
  User,
  AuthState,
  LoginCredentials,
  RegisterCredentials,
  AuthDialogMode,
  ValidationErrors
} from "./models";

// API Types
export type {
  ApiError,
  ApiResponse,
  RequestOptions,
  PaginationParams,
  SearchParams,
  PostsParams
} from "./api";

// API Utilities
export {
  ApiException,
  handleApiError,
  isApiError,
  validateResponse
} from "./api";

// Validation Utilities
export {
  isPostDto,
  isPostSummaryDto,
  isPaginatedResult,
  isValidDateString,
  isValidUrl,
  sanitizeString,
  sanitizeHtml
} from "./validators";