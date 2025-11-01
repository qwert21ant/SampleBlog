using WebApi.Models;

namespace WebApi.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int id, bool includeUnpublished = false);
    Task<IEnumerable<Post>> GetAllAsync(int page, int pageSize, bool? isPublished = null, bool includeUnpublished = false);
    Task<int> GetTotalCountAsync(bool? isPublished = null, bool includeUnpublished = false);
    Task<IEnumerable<Post>> GetPublishedAsync(int page, int pageSize, string? search = null);
    Task<int> GetPublishedCountAsync(string? search = null);
    Task<IEnumerable<Post>> GetRecentAsync(int count);
    Task<IEnumerable<Post>> SearchAsync(string query, int page, int pageSize);
    Task<int> GetSearchCountAsync(string query);
    Task<IEnumerable<Post>> GetByAuthorAsync(int authorId, int page, int pageSize);
    Task<int> GetByAuthorCountAsync(int authorId);
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
    Task<bool> DeleteAsync(int id);
}