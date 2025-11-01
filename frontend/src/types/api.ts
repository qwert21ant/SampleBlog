// API related types and utilities

export interface ApiError {
  message: string
  status: number
  details?: any
}

export interface ApiResponse<T> {
  data: T
  success: boolean
  message?: string
}

export interface RequestOptions {
  method?: "GET" | "POST" | "PUT" | "DELETE" | "PATCH"
  headers?: Record<string, string>
  body?: any
  timeout?: number
}

export interface PaginationParams {
  page?: number
  pageSize?: number
}

export interface SearchParams extends PaginationParams {
  query?: string
  search?: string
}

export interface PostsParams extends PaginationParams {
  category?: string
  author?: string
  tags?: string[]
  sortBy?: string
  sortDirection?: "asc" | "desc"
}

export class ApiException extends Error {
  public readonly status: number;
  public readonly details?: any;

  constructor(message: string, status: number, details?: any) {
    super(message);
    this.name = "ApiException";
    this.status = status;
    this.details = details;
  }
}

export const handleApiError = (error: unknown): never => {
  if (error instanceof ApiException) {
    throw error;
  }

  if (error instanceof Error) {
    throw new ApiException(error.message, 500);
  }

  throw new ApiException("An unknown error occurred", 500);
};

export const isApiError = (error: unknown): error is ApiException => {
  return error instanceof ApiException;
};

// Response validation utility
export const validateResponse = <T>(response: any, validator?: (data: any) => data is T): T => {
  if (validator && !validator(response)) {
    throw new ApiException("Invalid response format", 422, response);
  }
  return response;
};