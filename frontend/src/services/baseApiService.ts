import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { ApiException } from "@/types";

// Common HTTP service base class
export class BaseApiService {
  protected readonly client: AxiosInstance;

  constructor(baseUrl?: string) {
    this.client = axios.create({
      baseURL: baseUrl || import.meta.env.VITE_API_BASE_URL || "/api",
      timeout: 10000,
      headers: {
        "Content-Type": "application/json",
      },
    });

    // Request interceptor
    this.client.interceptors.request.use(
      (config) => {
        // Add auth token if available
        const token = localStorage.getItem("auth_token");
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        
        console.log(`Making ${config.method?.toUpperCase()} request to: ${config.url}`);
        return config;
      },
      (error) => {
        console.error("Request error:", error);
        return Promise.reject(error);
      }
    );

    // Response interceptor
    this.client.interceptors.response.use(
      (response: AxiosResponse) => {
        return response;
      },
      (error) => {
        let errorMessage = "An unknown error occurred";
        let statusCode = 500;
        let errorDetails: any = undefined;

        if (error.response) {
          // Server responded with error status
          statusCode = error.response.status;
          errorDetails = error.response.data;
          
          // Handle 401 unauthorized errors
          if (statusCode === 401) {
            this.handleUnauthorized();
          }
          
          if (errorDetails) {
            errorMessage = errorDetails.message || errorDetails.title || error.response.statusText || errorMessage;
          } else {
            errorMessage = error.response.statusText || errorMessage;
          }
        } else if (error.request) {
          // Request was made but no response received
          errorMessage = "No response from server";
          statusCode = 0;
        } else {
          // Something else happened
          errorMessage = error.message || errorMessage;
        }

        console.error("API request failed:", error);
        throw new ApiException(errorMessage, statusCode, errorDetails);
      }
    );
  }

  // Handle unauthorized (401) responses
  private handleUnauthorized(): void {
    // Clear stored auth data
    localStorage.removeItem("auth_token");
    localStorage.removeItem("user_data");
    
    // Remove auth header
    this.removeAuthHeader();
    
    // Redirect to home page (auth dialog will be shown)
    if (typeof window !== "undefined") {
      window.location.href = "/";
    }
  }

  // GET request
  protected async get<T>(endpoint: string, params?: Record<string, any>): Promise<T> {
    const response = await this.client.get<T>(endpoint, { params });
    return response.data;
  }

  // POST request
  protected async post<T, U = any>(endpoint: string, data?: U): Promise<T> {
    const response = await this.client.post<T>(endpoint, data);
    return response.data;
  }

  // PUT request
  protected async put<T, U = any>(endpoint: string, data?: U): Promise<T> {
    const response = await this.client.put<T>(endpoint, data);
    return response.data;
  }

  // DELETE request
  protected async delete<T>(endpoint: string): Promise<T> {
    const response = await this.client.delete<T>(endpoint);
    return response.data;
  }

  // PATCH request
  protected async patch<T, U = any>(endpoint: string, data?: U): Promise<T> {
    const response = await this.client.patch<T>(endpoint, data);
    return response.data;
  }

  // Method to set authorization header
  protected setAuthHeader(token: string): void {
    this.client.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  }

  // Method to remove authorization header
  protected removeAuthHeader(): void {
    delete this.client.defaults.headers.common["Authorization"];
  }

  // Method to get the axios instance for advanced usage
  protected getClient(): AxiosInstance {
    return this.client;
  }
}