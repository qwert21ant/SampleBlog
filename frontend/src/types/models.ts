// Frontend models - client-side interfaces for application use

export interface PublicPost {
  id: number
  title: string
  content: string
  subtitle: string
  author: string
  publishedAt: string
  imageUrl?: string
}

export interface PostCard {
  id: number
  title: string
  excerpt: string
  author: string
  publishedAt: string
  imageUrl?: string
}

export interface AdminPost {
  id: number
  title: string
  subtitle: string
  text: string
  mainImageUrl?: string
  isPublished: boolean
  createdAt: string
  updatedAt: string
  publishedAt?: string
  author: {
    id: number
    email: string
    username: string
  }
}

export interface PostFilters {
  search?: string
  category?: string
  author?: string
  tags?: string[]
  dateFrom?: string
  dateTo?: string
}

export interface PostSortOptions {
  sortBy: "publishedAt" | "title" | "author" | "readTime"
  sortDirection: "asc" | "desc"
}

export interface PaginationOptions {
  page: number
  pageSize: number
}

export interface PostSearchResult {
  posts: PublicPost[]
  totalCount: number
  hasMore: boolean
  currentPage: number
  totalPages: number
}

export interface BlogStats {
  totalPosts: number
  publishedPosts: number
  draftPosts: number
}

// Authentication models
export interface User {
  id: number
  email: string
  username: string
}

export interface AuthState {
  isAuthenticated: boolean
  user: User | null
  token: string | null
  loading: boolean
  error: string | null
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterCredentials {
  email: string
  password: string
  confirmPassword: string
  username: string
}

export interface AuthDialogMode {
  mode: "login" | "register"
  isOpen: boolean
}

export interface ValidationErrors {
  email?: string
  password?: string
  confirmPassword?: string
  username?: string
  general?: string
}