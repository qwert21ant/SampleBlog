using WebApi.Models;

namespace WebApi.Interfaces;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetByAuthorAsync(int authorId, int page, int pageSize, bool? isPublished = null);
    Task<Post?> GetByIdAsync(int id, bool includeUnpublished = false);
    Task<int> GetTotalCountAsync(bool? isPublished = null);
    Task<int> GetByAuthorCountAsync(int authorId, bool? isPublished = null);
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
    Task<bool> DeleteAsync(int id);

    Task<IEnumerable<Post>> GetAllAsync(int page, int pageSize, bool? isPublished = null);
    Task<IEnumerable<Post>> GetRecentAsync(int count);
}