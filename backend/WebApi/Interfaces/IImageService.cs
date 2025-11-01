using WebApi.Models;

namespace WebApi.Interfaces;

public interface IImageService
{
    Task<Image> UploadImageAsync(int postId, IFormFile file, string? altText = null);
    Task<(byte[] Data, string ContentType, string FileName)?> GetImageAsync(int id);
    Task<bool> DeleteImageAsync(int id);
    Task<IEnumerable<Image>> GetImagesByPostAsync(int postId);
}