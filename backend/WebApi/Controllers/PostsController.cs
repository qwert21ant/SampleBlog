using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Models.DTOs;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostService postService, ILogger<PostsController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PostSummaryDto>>> GetPublishedPosts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        _logger.LogInformation("Fetching published posts - Page: {Page}, PageSize: {PageSize}, Search: {Search}", 
            page, pageSize, search);

        var result = await _postService.GetPublishedPostsAsync(page, pageSize, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPublishedPost(int id)
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

    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResult<PostSummaryDto>>> SearchPosts(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Search query is required");
        }

        _logger.LogInformation("Searching posts with query: {Query}, Page: {Page}, PageSize: {PageSize}", 
            query, page, pageSize);
        
        var result = await _postService.SearchPostsAsync(query, page, pageSize);
        return Ok(result);
    }

    [HttpGet("by-author/{authorId}")]
    public async Task<ActionResult<PaginatedResult<PostSummaryDto>>> GetPostsByAuthor(
        int authorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching posts by author ID: {AuthorId}, Page: {Page}, PageSize: {PageSize}", 
            authorId, page, pageSize);
        
        var result = await _postService.GetPostsByAuthorAsync(authorId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetPostStats()
    {
        _logger.LogInformation("Fetching post statistics");
        
        var stats = await _postService.GetPostStatsAsync();
        return Ok(stats);
    }
}