using WebApi.Exceptions;
using WebApi.Interfaces;
using WebApi.Models.DTOs;

namespace WebApi.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IImageService _imageService;
    private readonly ILogger<PostService> _logger;

    public PostService(IPostRepository postRepository, IImageService imageService, ILogger<PostService> logger)
    {
        _postRepository = postRepository;
        _imageService = imageService;
        _logger = logger;
    }

    // Admin methods
    public async Task<PaginatedResult<PostDto>> GetPostsAsync(int page, int pageSize, bool? isPublished = null)
    {
        _logger.LogInformation("Fetching posts - Page: {Page}, PageSize: {PageSize}, IsPublished: {IsPublished}", 
            page, pageSize, isPublished);
        
        var posts = await _postRepository.GetAllAsync(page, pageSize, isPublished);
        var totalCount = await _postRepository.GetTotalCountAsync(isPublished);
        
        return new PaginatedResult<PostDto>
        {
            Items = posts.Select(p => p.ToDto()),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<PostDto>> GetPostsByAuthorAsync(int authorId, int page, int pageSize, bool? isPublished = null)
    {
        _logger.LogInformation("Fetching posts by author: {AuthorId} (Admin), Page: {Page}, PageSize: {PageSize}, IsPublished: {IsPublished}", 
            authorId, page, pageSize, isPublished);
        
        var posts = await _postRepository.GetByAuthorAsync(authorId, page, pageSize, isPublished);
        var totalCount = await _postRepository.GetByAuthorCountAsync(authorId, isPublished);
        
        return new PaginatedResult<PostDto>
        {
            Items = posts.Select(p => p.ToDto()),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PostDto> GetPostByIdAsync(int id, bool includeUnpublished = false)
    {
        _logger.LogInformation("Fetching post by ID: {PostId}, IncludeUnpublished: {IncludeUnpublished}", id, includeUnpublished);

        var post = await _postRepository.GetByIdAsync(id, includeUnpublished);
        if (post == null)
        {
            throw new PostNotFoundException(id);
        }

        return post.ToDto();
    }
    
    public async Task<PostStatsDto> GetPostStatsByAuthorAsync(int authorId)
    {
        _logger.LogInformation("Fetching post statistics for author: {AuthorId}", authorId);
        
        var totalCount = await _postRepository.GetByAuthorCountAsync(authorId, null);
        var publishedCount = await _postRepository.GetByAuthorCountAsync(authorId, true);
        var draftCount = await _postRepository.GetByAuthorCountAsync(authorId, false);
        
        return new PostStatsDto
        {
            TotalPosts = totalCount,
            PublishedPosts = publishedCount,
            DraftPosts = draftCount
        };
    }

    public async Task<PostDto> CreatePostAsync(CreatePostRequest request, int authorId)
    {
        _logger.LogInformation("Creating new post: {Title} by author: {AuthorId}", request.Title, authorId);
        
        var post = request.ToEntity(authorId);
        var createdPost = await _postRepository.CreateAsync(post);
        
        return createdPost.ToDto();
    }

    public async Task<PostDto> UpdatePostAsync(int id, UpdatePostRequest request)
    {
        _logger.LogInformation("Updating post: {PostId}", id);
        
        var existingPost = await _postRepository.GetByIdAsync(id, includeUnpublished: true);
        if (existingPost == null)
        {
            throw new PostNotFoundException(id);
        }
        
        existingPost.UpdateFromDto(request);
        var updatedPost = await _postRepository.UpdateAsync(existingPost);
        
        return updatedPost.ToDto();
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        _logger.LogInformation("Deleting post: {PostId}", id);
        
        var deleted = await _postRepository.DeleteAsync(id);
        if (!deleted)
        {
            throw new PostNotFoundException(id);
        }
        
        return deleted;
    }

    public async Task<PostDto> PublishPostAsync(int id)
    {
        _logger.LogInformation("Publishing post: {PostId}", id);
        
        var post = await _postRepository.GetByIdAsync(id, includeUnpublished: true);
        if (post == null)
        {
            throw new PostNotFoundException(id);
        }
        
        post.IsPublished = true;
        if (post.PublishedAt == null)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        
        var updatedPost = await _postRepository.UpdateAsync(post);
        return updatedPost.ToDto();
    }

    public async Task<PostDto> UnpublishPostAsync(int id)
    {
        _logger.LogInformation("Unpublishing post: {PostId}", id);

        var post = await _postRepository.GetByIdAsync(id, includeUnpublished: true);
        if (post == null)
        {
            throw new PostNotFoundException(id);
        }

        post.IsPublished = false;
        post.PublishedAt = null;

        var updatedPost = await _postRepository.UpdateAsync(post);
        return updatedPost.ToDto();
    }
    
    public async Task AddImageToPostAsync(int id, IFormFile image)
    {
        _logger.LogInformation("Adding image to post ID: {PostId}", id);
        
        var post = await _postRepository.GetByIdAsync(id, includeUnpublished: true);
        if (post == null)
        {
            throw new PostNotFoundException(id);
        }

        // Upload new image associated with the post
        await _imageService.UploadImageAsync(id, image);
    }

    public async Task DeletePostImagesAsync(int id)
    {
        _logger.LogInformation("Deleting all images for post ID: {PostId}", id);
        
        var post = await _postRepository.GetByIdAsync(id, includeUnpublished: true);
        if (post == null)
        {
            throw new PostNotFoundException(id);
        }

        // Get all images for the post and delete them
        var images = await _imageService.GetImagesByPostAsync(id);
        foreach (var image in images)
        {
            await _imageService.DeleteImageAsync(image.Id);
        }
    }

    // Public methods
    public async Task<PaginatedResult<PostSummaryDto>> GetPublishedPostsAsync(int page, int pageSize)
    {
        _logger.LogInformation("Fetching published posts - Page: {Page}, PageSize: {PageSize}", page, pageSize);
        
        var posts = await _postRepository.GetAllAsync(page, pageSize, true);
        var totalCount = await _postRepository.GetTotalCountAsync(true);
        
        return new PaginatedResult<PostSummaryDto>
        {
            Items = posts.Select(p => p.ToSummaryDto()),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PostPublicDto> GetPublishedPostByIdAsync(int id)
    {
        _logger.LogInformation("Fetching published post by ID: {PostId}", id);
        
        var post = await _postRepository.GetByIdAsync(id, includeUnpublished: false);
        if (post == null || !post.IsPublished)
        {
            throw new PostNotFoundException(id);
        }
        
        return post.ToDto().ToPublicDto();
    }

    public async Task<IEnumerable<PostSummaryDto>> GetRecentPostsAsync(int count)
    {
        _logger.LogInformation("Fetching {Count} recent posts", count);
        
        var posts = await _postRepository.GetRecentAsync(count);
        
        return posts.Select(p => p.ToSummaryDto());
    }
}