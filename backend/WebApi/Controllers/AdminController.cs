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
    private readonly ILogger<AdminController> _logger;

    public AdminController(IPostService postService, ILogger<AdminController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpGet("posts")]
    public async Task<ActionResult<PaginatedResult<PostDto>>> GetAllPosts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isPublished = null)
    {
        _logger.LogInformation("Fetching posts - Page: {Page}, PageSize: {PageSize}", page, pageSize);
        
        var result = await _postService.GetPostsAsync(page, pageSize, isPublished, includeUnpublished: true);
        return Ok(result);
    }

    [HttpGet("posts/{id}")]
    public async Task<ActionResult<PostDto>> GetPost(int id)
    {
        _logger.LogInformation("Fetching post with ID: {PostId}", id);
        
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
        
        var post = await _postService.UpdatePostAsync(id, request);
        return Ok(post);
    }

    [HttpDelete("posts/{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        _logger.LogInformation("Deleting post with ID {PostId}", id);
        
        await _postService.DeletePostAsync(id);
        return NoContent();
    }

    [HttpPatch("posts/{id}/publish")]
    public async Task<ActionResult<PostDto>> PublishPost(int id)
    {
        _logger.LogInformation("Publishing post with ID {PostId}", id);
        
        var post = await _postService.PublishPostAsync(id);
        return Ok(post);
    }

    [HttpPatch("posts/{id}/unpublish")]
    public async Task<ActionResult<PostDto>> UnpublishPost(int id)
    {
        _logger.LogInformation("Unpublishing post with ID {PostId}", id);
        
        var post = await _postService.UnpublishPostAsync(id);
        return Ok(post);
    }

    [HttpPost("posts/{id}/image")]
    public async Task<ActionResult> UploadPostImage(int id, [FromForm] ImageUploadRequest request)
    {
        _logger.LogInformation("Uploading image for post ID: {PostId}", id);
        
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

        await _postService.AddImageToPostAsync(id, request.Image);
        return Ok(new { message = "Image uploaded successfully" });
    }

    [HttpDelete("posts/{id}/images")]
    public async Task<ActionResult> DeletePostImages(int id)
    {
        _logger.LogInformation("Deleting all images for post ID: {PostId}", id);
        
        await _postService.DeletePostImagesAsync(id);
        return Ok(new { message = "Images deleted successfully" });
    }



    private int GetCurrentUserId()
    {
        // TODO: Extract user ID from JWT token claims
        // For now, return 1 - this will be implemented when authentication is set up
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst("sub");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
    }
}