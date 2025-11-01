using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Models.DTOs;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(IImageService imageService, ILogger<ImagesController> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetImage(int id)
    {
        _logger.LogInformation("Fetching image with ID: {ImageId}", id);
        
        var imageData = await _imageService.GetImageAsync(id);
        
        if (imageData == null)
        {
            return NotFound();
        }

        return File(imageData.Value.Data, imageData.Value.ContentType, imageData.Value.FileName);
    }

    [HttpPost("post/{postId}")]
    [Authorize]
    public async Task<ActionResult<object>> UploadImageForPost(int postId, [FromForm] ImageUploadRequest request)
    {
        _logger.LogInformation("Uploading image: {FileName} for post: {PostId}", request.Image.FileName, postId);
        
        var image = await _imageService.UploadImageAsync(postId, request.Image, request.AltText);
        
        return CreatedAtAction(nameof(GetImage), new { id = image.Id }, new
        {
            Id = image.Id,
            PostId = image.PostId,
            FileName = image.FileName,
            ContentType = image.ContentType,
            Size = image.Size,
            AltText = image.AltText,
            Url = $"/api/images/{image.Id}"
        });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteImage(int id)
    {
        _logger.LogInformation("Deleting image with ID: {ImageId}", id);
        
        await _imageService.DeleteImageAsync(id);
        return NoContent();
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<object>> GetImagesByPost(int postId)
    {
        _logger.LogInformation("Fetching images for post: {PostId}", postId);
        
        var images = await _imageService.GetImagesByPostAsync(postId);
        
        var result = images.Select(i => new
        {
            Id = i.Id,
            PostId = i.PostId,
            FileName = i.FileName,
            ContentType = i.ContentType,
            Size = i.Size,
            AltText = i.AltText,
            CreatedAt = i.CreatedAt,
            Url = $"/api/images/{i.Id}"
        });
        
        return Ok(result);
    }
}