// Type validation utilities

import type { PostDto, PostSummaryDto, PaginatedResult } from "./dtos";

// Type guards for runtime validation
export const isPostDto = (obj: any): obj is PostDto => {
  return obj &&
    typeof obj.id === "number" &&
    typeof obj.title === "string" &&
    typeof obj.content === "string" &&
    typeof obj.excerpt === "string" &&
    typeof obj.publishedAt === "string" &&
    typeof obj.author === "string";
};

export const isPostSummaryDto = (obj: any): obj is PostSummaryDto => {
  return obj &&
    typeof obj.id === "number" &&
    typeof obj.title === "string" &&
    typeof obj.excerpt === "string" &&
    typeof obj.publishedAt === "string" &&
    typeof obj.author === "string";
};

export const isPaginatedResult = <T>(obj: any, itemValidator?: (item: any) => item is T): obj is PaginatedResult<T> => {
  if (!obj || typeof obj !== "object") return false;
  
  const hasRequiredFields = 
    Array.isArray(obj.items) &&
    typeof obj.totalCount === "number" &&
    typeof obj.pageNumber === "number" &&
    typeof obj.pageSize === "number" &&
    typeof obj.totalPages === "number" &&
    typeof obj.hasNextPage === "boolean" &&
    typeof obj.hasPreviousPage === "boolean";
  
  if (!hasRequiredFields) return false;
  
  // Validate items if validator is provided
  if (itemValidator && obj.items.length > 0) {
    return obj.items.every(itemValidator);
  }
  
  return true;
};

// Date validation utilities
export const isValidDateString = (dateStr: string): boolean => {
  const date = new Date(dateStr);
  return !isNaN(date.getTime());
};

// URL validation
export const isValidUrl = (url: string): boolean => {
  try {
    new URL(url);
    return true;
  } catch {
    return false;
  }
};

// Sanitization utilities
export const sanitizeString = (str: string): string => {
  return str.trim().replace(/[<>]/g, "");
};

export const sanitizeHtml = (html: string): string => {
  // Basic HTML sanitization - in production use a proper library like DOMPurify
  return html
    .replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, "")
    .replace(/<iframe\b[^<]*(?:(?!<\/iframe>)<[^<]*)*<\/iframe>/gi, "")
    .replace(/on\w+="[^"]*"/gi, "");
};