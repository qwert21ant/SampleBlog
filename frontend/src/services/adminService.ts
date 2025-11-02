import { BaseApiService } from "./baseApiService";
import type {
  AdminPost,
  PostDto,
  PostStatsDto,
  CreatePostDto,
  UpdatePostDto,
  PaginatedResult,
  ImageDetailsDto
} from "@/types";

// Admin service for handling administrative operations
class AdminService extends BaseApiService {
  constructor() {
    super();
  }

  // Map PostDto to AdminPost
  private mapPostDtoToAdminPost(dto: PostDto): AdminPost {
    return {
      id: dto.id,
      title: dto.title,
      subtitle: dto.subtitle,
      text: dto.text,
      mainImageUrl: dto.mainImageUrl,
      isPublished: dto.isPublished,
      createdAt: dto.createdAt,
      updatedAt: dto.createdAt, // PostDto doesn't have updatedAt, using createdAt
      publishedAt: dto.publishedAt,
      author: dto.author
    };
  }

  // Get all posts (published and drafts) for admin
  async getAllPosts(params?: {
    page?: number;
    pageSize?: number;
    isPublished?: boolean;
  }): Promise<PaginatedResult<AdminPost>> {
    try {
      const queryParams = {
        page: params?.page || 1,
        pageSize: params?.pageSize || 10,
        ...(params?.isPublished !== undefined && { isPublished: params.isPublished })
      };

      const result = await this.get<PaginatedResult<PostDto>>("/admin/posts", queryParams);
      
      return {
        ...result,
        items: result.items.map(post => this.mapPostDtoToAdminPost(post))
      };
    } catch (error) {
      console.error("Failed to get all posts:", error);
      throw error;
    }
  }

  // Get a specific post by ID (for editing)
  async getPostById(id: number): Promise<AdminPost> {
    try {
      const post = await this.get<PostDto>(`/admin/posts/${id}`);
      return this.mapPostDtoToAdminPost(post);
    } catch (error) {
      console.error(`Failed to get post ${id}:`, error);
      throw error;
    }
  }

  // Create a new post
  async createPost(postData: CreatePostDto): Promise<AdminPost> {
    try {
      const post = await this.post<PostDto>("/admin/posts", postData);
      return this.mapPostDtoToAdminPost(post);
    } catch (error) {
      console.error("Failed to create post:", error);
      throw error;
    }
  }

  // Update an existing post
  async updatePost(id: number, postData: Partial<UpdatePostDto>): Promise<AdminPost> {
    try {
      const post = await this.put<PostDto>(`/admin/posts/${id}`, postData);
      return this.mapPostDtoToAdminPost(post);
    } catch (error) {
      console.error(`Failed to update post ${id}:`, error);
      throw error;
    }
  }

  // Publish a post
  async publishPost(id: number): Promise<AdminPost> {
    try {
      const post = await this.patch<PostDto>(`/admin/posts/${id}/publish`);
      return this.mapPostDtoToAdminPost(post);
    } catch (error) {
      console.error(`Failed to publish post ${id}:`, error);
      throw error;
    }
  }

  // Unpublish a post (convert to draft)
  async unpublishPost(id: number): Promise<AdminPost> {
    try {
      const post = await this.patch<PostDto>(`/admin/posts/${id}/unpublish`);
      return this.mapPostDtoToAdminPost(post);
    } catch (error) {
      console.error(`Failed to unpublish post ${id}:`, error);
      throw error;
    }
  }

  // Delete a post
  async deletePost(id: number): Promise<void> {
    try {
      await this.delete<void>(`/admin/posts/${id}`);
    } catch (error) {
      console.error(`Failed to delete post ${id}:`, error);
      throw error;
    }
  }

  // Get post statistics for admin dashboard
  async getPostStats(): Promise<PostStatsDto> {
    try {
      return await this.get<PostStatsDto>("/admin/stats");
    } catch (error) {
      console.error("Failed to get post stats:", error);
      throw error;
    }
  }

  // Upload image for a specific post
  async uploadImageForPost(postId: number, file: File, altText?: string): Promise<ImageDetailsDto> {
    try {
      const formData = new FormData();
      formData.append("image", file);
      if (altText) {
        formData.append("altText", altText);
      }

      const response = await this.getClient().post<ImageDetailsDto>(`/admin/posts/${postId}/images`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      return response.data;
    } catch (error) {
      console.error("Failed to upload image:", error);
      throw error;
    }
  }

  // Get images for a specific post
  async getImagesByPost(postId: number): Promise<ImageDetailsDto[]> {
    try {
      return await this.get<ImageDetailsDto[]>(`/admin/posts/${postId}/images`);
    } catch (error) {
      console.error(`Failed to get images for post ${postId}:`, error);
      throw error;
    }
  }

  // Delete an image
  async deleteImage(imageId: number): Promise<void> {
    try {
      await this.delete<void>(`/admin/images/${imageId}`);
    } catch (error) {
      console.error(`Failed to delete image ${imageId}:`, error);
      throw error;
    }
  }

  // Get image URL for admin context
  getImageUrl(imageId: number): string {
    const baseURL = this.client.defaults.baseURL || "/api";
    return `${baseURL}/admin/images/${imageId}`;
  }

  // Fetch image blob with authentication (for displaying in authenticated contexts)
  async fetchImageBlob(imageId: number): Promise<Blob> {
    try {
      const response = await this.getClient().get(`/admin/images/${imageId}`, {
        responseType: "blob"
      });
      
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch image ${imageId}:`, error);
      throw error;
    }
  }
}

// Export singleton instance
export const adminService = new AdminService();
export default adminService;