using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories;

public class ImageRepository : IImageRepository
{
    private readonly BlogDbContext _context;
    private readonly ILogger<ImageRepository> _logger;

    public ImageRepository(BlogDbContext context, ILogger<ImageRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Image?> GetByIdAsync(int id)
    {
        _logger.LogDebug("Getting image by ID: {ImageId}", id);
        return await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Image> CreateAsync(Image image)
    {
        _logger.LogDebug("Creating new image: {FileName}", image.FileName);
        
        image.CreatedAt = DateTime.UtcNow;
        image.UpdatedAt = DateTime.UtcNow;
        
        _context.Images.Add(image);
        await _context.SaveChangesAsync();
        
        return image;
    }

    public async Task<Image> UpdateAsync(Image image)
    {
        _logger.LogDebug("Updating image: {ImageId}", image.Id);
        
        image.UpdatedAt = DateTime.UtcNow;
        
        _context.Images.Update(image);
        await _context.SaveChangesAsync();
        
        return image;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogDebug("Deleting image: {ImageId}", id);
        
        var image = await GetByIdAsync(id);
        if (image == null)
        {
            return false;
        }
        
        _context.Images.Remove(image);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<Image>> GetByPostIdAsync(int postId)
    {
        _logger.LogDebug("Getting images for post: {PostId}", postId);
        
        return await _context.Images
            .Where(i => i.PostId == postId)
            .OrderBy(i => i.CreatedAt)
            .ToListAsync();
    }
}