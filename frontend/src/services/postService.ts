import { BaseApiService } from "./baseApiService";
import type {
  PostSummaryDto,
  PostPublicDto,
  PaginatedResult,
  PublicPost,
  PostsParams,
  ImageDetailsDto
} from "@/types";

// Post service for handling blog posts API calls
class PostService extends BaseApiService {
  constructor() {
    super();
  }

  // Map PostSummaryDto to PublicPost
  private mapSummaryToPublicPost(dto: PostSummaryDto): PublicPost {
    return {
      id: dto.id,
      title: dto.title,
      content: dto.subtitle, // Use subtitle as content for summary
      subtitle: dto.subtitle,
      author: dto.authorUsername,
      publishedAt: dto.publishedAt,
      imageUrl: dto.mainImageUrl
    };
  }

  // Map PostPublicDto to PublicPost
  private mapPostPublicToPublicPost(dto: PostPublicDto): PublicPost {
    return {
      id: dto.id,
      title: dto.title,
      content: dto.text,
      subtitle: dto.subtitle,
      author: dto.authorUsername,
      publishedAt: dto.publishedAt,
      imageUrl: dto.mainImageUrl
    };
  }

  // Get recent posts using backend /posts/recent endpoint
  async getLatestPosts(limit: number = 10): Promise<PublicPost[]> {
    try {
      const posts = await this.get<PostSummaryDto[]>("/posts/recent", { count: limit });
      return posts.map(post => this.mapSummaryToPublicPost(post));
    } catch (error) {
      console.error("Failed to fetch latest posts:", error);
      throw error;
    }
  }

  // Get published posts with pagination using backend /posts endpoint
  async getPublishedPosts(params?: PostsParams): Promise<PaginatedResult<PublicPost>> {
    try {
      const result = await this.get<PaginatedResult<PostSummaryDto>>("/posts", params);
      
      return {
        ...result,
        items: result.items.map(post => this.mapSummaryToPublicPost(post))
      };
    } catch (error) {
      console.error("Failed to fetch published posts:", error);
      throw error;
    }
  }

  // Get a single post by ID using backend /posts/{id} endpoint
  async getPostById(id: number): Promise<PublicPost> {
    try {
      const post = await this.get<PostPublicDto>(`/posts/${id}`);
      return this.mapPostPublicToPublicPost(post);
    } catch (error) {
      console.error(`Failed to fetch post ${id}:`, error);
      throw error;
    }
  }

  // Get images for a specific post (public)
  async getImagesByPost(postId: number): Promise<ImageDetailsDto[]> {
    try {
      return await this.get<ImageDetailsDto[]>(`/posts/${postId}/images`);
    } catch (error) {
      console.error(`Failed to get images for post ${postId}:`, error);
      throw error;
    }
  }

  // Get public image URL
  getImageUrl(imageId: number): string {
    const baseURL = this.client.defaults.baseURL || '/api';
    return `${baseURL}/posts/images/${imageId}`;
  }
}

// Export singleton instance
export const postService = new PostService();
export default postService;