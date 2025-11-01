using WebApi.Exceptions;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Models.DTOs;

namespace WebApi.Services;

public class ImageService : IImageService
{
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<ImageService> _logger;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private readonly string[] _allowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

    public ImageService(IImageRepository imageRepository, ILogger<ImageService> logger)
    {
        _imageRepository = imageRepository;
        _logger = logger;
    }

    public async Task<Image> UploadImageAsync(int postId, IFormFile file, string? altText = null)
    {
        _logger.LogInformation("Uploading image: {FileName} for post: {PostId}", file.FileName, postId);

        // Validate file
        ValidateFile(file);

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var image = new Image
        {
            PostId = postId,
            Data = memoryStream.ToArray(),
            ContentType = file.ContentType,
            FileName = file.FileName,
            AltText = altText,
            Size = file.Length
        };

        return await _imageRepository.CreateAsync(image);
    }

    public async Task<(byte[] Data, string ContentType, string FileName)?> GetImageAsync(int id)
    {
        _logger.LogInformation("Getting image: {ImageId}", id);

        var image = await _imageRepository.GetByIdAsync(id);
        if (image == null)
        {
            return null;
        }

        return (image.Data, image.ContentType, image.FileName);
    }

    public async Task<ImageDetailsDto?> GetImageDetailsAsync(int id)
    {
        _logger.LogInformation("Getting image details: {ImageId}", id);

        var image = await _imageRepository.GetByIdAsync(id);
        if (image == null)
        {
            return null;
        }

        return new ImageDetailsDto
        {
            Id = image.Id,
            PostId = image.PostId,
            FileName = image.FileName,
            ContentType = image.ContentType,
            Size = image.Size,
            AltText = image.AltText,
            CreatedAt = image.CreatedAt,
            Url = $"/api/images/{image.Id}" // Default URL, can be overridden by controllers
        };
    }

    public async Task<bool> DeleteImageAsync(int id)
    {
        _logger.LogInformation("Deleting image: {ImageId}", id);

        var deleted = await _imageRepository.DeleteAsync(id);
        if (!deleted)
        {
            throw new KeyNotFoundException($"Image with ID {id} not found");
        }

        return deleted;
    }

    public async Task<IEnumerable<Image>> GetImagesByPostAsync(int postId)
    {
        _logger.LogInformation("Getting images for post: {PostId}", postId);
        return await _imageRepository.GetByPostIdAsync(postId);
    }

    public async Task<IEnumerable<ImageDetailsDto>> GetImageDetailsByPostAsync(int postId)
    {
        _logger.LogInformation("Getting image details for post: {PostId}", postId);
        
        var images = await _imageRepository.GetByPostIdAsync(postId);
        
        return images.Select(image => new ImageDetailsDto
        {
            Id = image.Id,
            PostId = image.PostId,
            FileName = image.FileName,
            ContentType = image.ContentType,
            Size = image.Size,
            AltText = image.AltText,
            CreatedAt = image.CreatedAt,
            Url = $"/api/images/{image.Id}" // Default URL, can be overridden by controllers
        });
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("File is required");
        }

        if (file.Length > _maxFileSize)
        {
            throw new BadRequestException($"File size exceeds maximum allowed size of {_maxFileSize / 1024 / 1024}MB");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new BadRequestException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}");
        }

        if (!_allowedContentTypes.Contains(file.ContentType))
        {
            throw new BadRequestException($"Content type '{file.ContentType}' is not allowed. Allowed types: {string.Join(", ", _allowedContentTypes)}");
        }
    }
}