using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Exceptions;
using WebApi.Interfaces;
using WebApi.Models.DTOs;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication
public class AdminController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IImageService _imageService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IPostService postService, IImageService imageService, ILogger<AdminController> logger)
    {
        _postService = postService;
        _imageService = imageService;
        _logger = logger;
    }

    [HttpGet("posts")]
    public async Task<ActionResult<PaginatedResult<PostDto>>> GetAllPosts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isPublished = null)
    {
        _logger.LogInformation("Fetching posts - Page: {Page}, PageSize: {PageSize}", page, pageSize);
        
        var currentUserId = GetCurrentUserId();
        var result = await _postService.GetPostsByAuthorAsync(currentUserId, page, pageSize);
        
        return Ok(result);
    }

    [HttpGet("posts/{id}")]
    public async Task<ActionResult<PostDto>> GetPost(int id)
    {
        _logger.LogInformation("Fetching post with ID: {PostId}", id);
        
        await ValidatePostOwnershipAsync(id);
        var post = await _postService.GetPostByIdAsync(id, includeUnpublished: true);
        return Ok(post);
    }

    [HttpPost("posts")]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostRequest request)
    {
        _logger.LogInformation("Creating new post: {Title}", request.Title);
        
        var userId = GetCurrentUserId();
        var post = await _postService.CreatePostAsync(request, userId);
        
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    [HttpPut("posts/{id}")]
    public async Task<ActionResult<PostDto>> UpdatePost(int id, [FromBody] UpdatePostRequest request)
    {
        _logger.LogInformation("Updating post with ID {PostId}", id);
        
        await ValidatePostOwnershipAsync(id);
        var post = await _postService.UpdatePostAsync(id, request);
        return Ok(post);
    }

    [HttpDelete("posts/{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        _logger.LogInformation("Deleting post with ID {PostId}", id);
        
        await ValidatePostOwnershipAsync(id);
        await _postService.DeletePostAsync(id);
        return NoContent();
    }

    [HttpPatch("posts/{id}/publish")]
    public async Task<ActionResult<PostDto>> PublishPost(int id)
    {
        _logger.LogInformation("Publishing post with ID {PostId}", id);
        
        await ValidatePostOwnershipAsync(id);
        var post = await _postService.PublishPostAsync(id);
        return Ok(post);
    }

    [HttpPatch("posts/{id}/unpublish")]
    public async Task<ActionResult<PostDto>> UnpublishPost(int id)
    {
        _logger.LogInformation("Unpublishing post with ID {PostId}", id);
        
        await ValidatePostOwnershipAsync(id);
        var post = await _postService.UnpublishPostAsync(id);
        return Ok(post);
    }

    [HttpPost("posts/{postId}/images")]
    public async Task<ActionResult<ImageDetailsDto>> UploadImageForPost(int postId, [FromForm] ImageUploadRequest request)
    {
        _logger.LogInformation("Uploading image: {FileName} for post: {PostId}", request.Image?.FileName, postId);
        
        await ValidatePostOwnershipAsync(postId);
        
        if (request.Image == null || request.Image.Length == 0)
        {
            return BadRequest("No image file provided");
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(request.Image.ContentType.ToLower()))
        {
            return BadRequest("Only JPEG, PNG, GIF, and WebP images are allowed");
        }

        // Validate file size (max 5MB)
        if (request.Image.Length > 5 * 1024 * 1024)
        {
            return BadRequest("Image size must be less than 5MB");
        }

        var image = await _imageService.UploadImageAsync(postId, request.Image, request.AltText);
        
        return Ok(new ImageDetailsDto
        {
            Id = image.Id,
            PostId = image.PostId,
            FileName = image.FileName,
            ContentType = image.ContentType,
            Size = image.Size,
            AltText = image.AltText,
            CreatedAt = image.CreatedAt,
            Url = $"/api/admin/images/{image.Id}"
        });
    }

    [HttpGet("images/{imageId}")]
    public async Task<IActionResult> GetImage(int imageId)
    {
        _logger.LogInformation("Fetching image with ID: {ImageId}", imageId);

        await ValidateImageOwnershipAsync(imageId);
        
        var imageData = await _imageService.GetImageAsync(imageId);
        
        if (imageData == null)
        {
            return NotFound();
        }

        return File(imageData.Value.Data, imageData.Value.ContentType, imageData.Value.FileName);
    }

    [HttpGet("posts/{postId}/images")]
    public async Task<ActionResult<IEnumerable<ImageDetailsDto>>> GetImagesByPost(int postId)
    {
        _logger.LogInformation("Fetching images for post: {PostId}", postId);
        
        await ValidatePostOwnershipAsync(postId);
        var images = await _imageService.GetImageDetailsByPostAsync(postId);
        
        // Update URLs to use admin endpoint
        var result = images.Select(i => new ImageDetailsDto
        {
            Id = i.Id,
            PostId = i.PostId,
            FileName = i.FileName,
            ContentType = i.ContentType,
            Size = i.Size,
            AltText = i.AltText,
            CreatedAt = i.CreatedAt,
            Url = $"/api/admin/images/{i.Id}"
        });
        
        return Ok(result);
    }

    [HttpDelete("images/{id}")]
    public async Task<IActionResult> DeleteImage(int id)
    {
        _logger.LogInformation("Deleting image with ID: {ImageId}", id);
        
        await ValidateImageOwnershipAsync(id);
        var deleted = await _imageService.DeleteImageAsync(id);
        
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<ActionResult<PostStatsDto>> GetPostStats()
    {
        _logger.LogInformation("Fetching post statistics for current user");
        
        var currentUserId = GetCurrentUserId();
        var stats = await _postService.GetPostStatsByAuthorAsync(currentUserId);
        
        return Ok(stats);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst("sub");
        
        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException("User ID not found in JWT token");
        }
        
        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID in JWT token");
        }
        
        return userId;
    }

    private async Task ValidatePostOwnershipAsync(int postId)
    {
        var currentUserId = GetCurrentUserId();
        var post = await _postService.GetPostByIdAsync(postId, includeUnpublished: true);
        
        if (post.Author.Id != currentUserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to access this post");
        }
    }

    private async Task ValidateImageOwnershipAsync(int imageId)
    {
        // Get the image details including postId
        var imageDetails = await _imageService.GetImageDetailsAsync(imageId);
        if (imageDetails == null)
        {
            throw new FileNotFoundException("Image not found");
        }
        
        // Now validate that the post belongs to the current user
        await ValidatePostOwnershipAsync(imageDetails.PostId);
    }
}