using WebApi.Models;

namespace WebApi.Interfaces;

public interface IImageRepository
{
    Task<Image?> GetByIdAsync(int id);
    Task<Image> CreateAsync(Image image);
    Task<Image> UpdateAsync(Image image);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Image>> GetByPostIdAsync(int postId);
}