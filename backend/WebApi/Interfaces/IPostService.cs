using WebApi.Models.DTOs;

namespace WebApi.Interfaces;

public interface IPostService
{
    // Admin methods
    Task<PaginatedResult<PostDto>> GetPostsAsync(int page, int pageSize, bool? isPublished = null, bool includeUnpublished = false);
    Task<PostDto> GetPostByIdAsync(int id, bool includeUnpublished = false);
    Task<PostDto> CreatePostAsync(CreatePostRequest request, int authorId);
    Task<PostDto> UpdatePostAsync(int id, UpdatePostRequest request);
    Task<bool> DeletePostAsync(int id);
    Task<PostDto> PublishPostAsync(int id);
    Task<PostDto> UnpublishPostAsync(int id);
    
    // Public methods
    Task<PaginatedResult<PostSummaryDto>> GetPublishedPostsAsync(int page, int pageSize, string? search = null);
    Task<PostDto> GetPublishedPostByIdAsync(int id);
    Task<IEnumerable<PostSummaryDto>> GetRecentPostsAsync(int count);
    Task<PaginatedResult<PostSummaryDto>> SearchPostsAsync(string query, int page, int pageSize);
    Task<PaginatedResult<PostSummaryDto>> GetPostsByAuthorAsync(int authorId, int page, int pageSize);
    Task<object> GetPostStatsAsync();
    Task AddImageToPostAsync(int id, IFormFile image);
    Task DeletePostImagesAsync(int id);
}