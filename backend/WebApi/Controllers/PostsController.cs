using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Models.DTOs;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IImageService _imageService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostService postService, IImageService imageService, ILogger<PostsController> logger)
    {
        _postService = postService;
        _imageService = imageService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PostSummaryDto>>> GetPublishedPosts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching published posts - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var result = await _postService.GetPublishedPostsAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostPublicDto>> GetPublishedPost(int id)
    {
        _logger.LogInformation("Fetching published post with ID: {PostId}", id);
        
        var post = await _postService.GetPublishedPostByIdAsync(id);
        return Ok(post);
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<PostSummaryDto>>> GetRecentPosts([FromQuery] int count = 5)
    {
        _logger.LogInformation("Fetching {Count} recent posts", count);
        
        var posts = await _postService.GetRecentPostsAsync(count);
        return Ok(posts);
    }

    [HttpGet("images/{id}")]
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

    [HttpGet("{postId}/images")]
    public async Task<ActionResult<IEnumerable<ImageDetailsDto>>> GetImagesByPost(int postId)
    {
        _logger.LogInformation("Fetching images for post: {PostId}", postId);
        
        var images = await _imageService.GetImageDetailsByPostAsync(postId);
        
        // Update URLs to use posts endpoint
        var result = images.Select(i => new ImageDetailsDto
        {
            Id = i.Id,
            PostId = i.PostId,
            FileName = i.FileName,
            ContentType = i.ContentType,
            Size = i.Size,
            AltText = i.AltText,
            CreatedAt = i.CreatedAt,
            Url = $"/api/posts/images/{i.Id}"
        });
        
        return Ok(result);
    }
}