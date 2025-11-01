using WebApi.Models.DTOs;

namespace WebApi.Interfaces;

public interface IPostService
{
    // Admin methods
    Task<PaginatedResult<PostDto>> GetPostsByAuthorAsync(int authorId, int page, int pageSize, bool? isPublished = null);
    Task<PostDto> GetPostByIdAsync(int id, bool includeUnpublished = false);
    Task<PostStatsDto> GetPostStatsByAuthorAsync(int authorId);
    Task<PostDto> CreatePostAsync(CreatePostRequest request, int authorId);
    Task<PostDto> UpdatePostAsync(int id, UpdatePostRequest request);
    Task<bool> DeletePostAsync(int id);
    Task<PostDto> PublishPostAsync(int id);
    Task<PostDto> UnpublishPostAsync(int id);
    Task AddImageToPostAsync(int id, IFormFile image);
    Task DeletePostImagesAsync(int id);
    
    // Public methods
    Task<PaginatedResult<PostSummaryDto>> GetPublishedPostsAsync(int page, int pageSize);
    Task<PostPublicDto> GetPublishedPostByIdAsync(int id);
    Task<IEnumerable<PostSummaryDto>> GetRecentPostsAsync(int count);
}