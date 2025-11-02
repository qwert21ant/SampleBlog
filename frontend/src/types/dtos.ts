// Backend DTOs - matching the server-side data transfer objects

import { UserDto } from "./auth";

export interface PostSummaryDto {
  id: number
  title: string
  subtitle: string
  mainImageUrl?: string
  createdAt: string
  publishedAt: string
  authorUsername: string
}

export interface PostDto {
  id: number
  title: string
  subtitle: string
  text: string
  mainImageUrl?: string
  isPublished: boolean
  createdAt: string
  publishedAt: string
  author: UserDto
}

export interface PostPublicDto {
  id: number
  title: string
  subtitle: string
  text: string
  mainImageUrl?: string
  isPublished: boolean
  createdAt: string
  publishedAt: string
  authorUsername: string
}

export interface CreatePostDto {
  title: string
  subtitle: string
  text: string
  isPublished: boolean
}

export interface UpdatePostDto {
  title: string
  subtitle: string
  text: string
  isPublished: boolean
}

export interface PaginatedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface PostStatsDto {
  totalPosts: number
  publishedPosts: number
  draftPosts: number
}

export interface ImageDetailsDto {
  id: number
  postId: number
  fileName: string
  contentType: string
  size: number
  altText?: string
  createdAt: string
  url: string
}