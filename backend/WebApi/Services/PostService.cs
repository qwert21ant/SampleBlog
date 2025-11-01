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
    public async Task<PaginatedResult<PostDto>> GetPostsAsync(int page, int pageSize, bool? isPublished = null, bool includeUnpublished = false)
    {
        _logger.LogInformation("Fetching posts - Page: {Page}, PageSize: {PageSize}, IsPublished: {IsPublished}, IncludeUnpublished: {IncludeUnpublished}", 
            page, pageSize, isPublished, includeUnpublished);
        
        var posts = await _postRepository.GetAllAsync(page, pageSize, isPublished, includeUnpublished);
        var totalCount = await _postRepository.GetTotalCountAsync(isPublished, includeUnpublished);
        
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

    // Public methods
    public async Task<PaginatedResult<PostSummaryDto>> GetPublishedPostsAsync(int page, int pageSize, string? search = null)
    {
        _logger.LogInformation("Fetching published posts - Page: {Page}, PageSize: {PageSize}, Search: {Search}", page, pageSize, search);
        
        var posts = await _postRepository.GetPublishedAsync(page, pageSize, search);
        var totalCount = await _postRepository.GetPublishedCountAsync(search);
        
        return new PaginatedResult<PostSummaryDto>
        {
            Items = posts.Select(p => p.ToSummaryDto()),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PostDto> GetPublishedPostByIdAsync(int id)
    {
        _logger.LogInformation("Fetching published post by ID: {PostId}", id);
        
        var post = await _postRepository.GetByIdAsync(id, includeUnpublished: false);
        if (post == null || !post.IsPublished)
        {
            throw new PostNotFoundException(id);
        }
        
        return post.ToDto();
    }

    public async Task<IEnumerable<PostSummaryDto>> GetRecentPostsAsync(int count)
    {
        _logger.LogInformation("Fetching {Count} recent posts", count);
        
        var posts = await _postRepository.GetRecentAsync(count);
        
        return posts.Select(p => p.ToSummaryDto());
    }

    public async Task<PaginatedResult<PostSummaryDto>> SearchPostsAsync(string query, int page, int pageSize)
    {
        _logger.LogInformation("Searching posts with query: {Query}", query);
        
        var posts = await _postRepository.SearchAsync(query, page, pageSize);
        var totalCount = await _postRepository.GetSearchCountAsync(query);
        
        return new PaginatedResult<PostSummaryDto>
        {
            Items = posts.Select(p => p.ToSummaryDto()),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<PostSummaryDto>> GetPostsByAuthorAsync(int authorId, int page, int pageSize)
    {
        _logger.LogInformation("Fetching posts by author: {AuthorId}", authorId);
        
        var posts = await _postRepository.GetByAuthorAsync(authorId, page, pageSize);
        var totalCount = await _postRepository.GetByAuthorCountAsync(authorId);
        
        return new PaginatedResult<PostSummaryDto>
        {
            Items = posts.Select(p => p.ToSummaryDto()),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<object> GetPostStatsAsync()
    {
        _logger.LogInformation("Fetching post statistics");
        
        var totalCount = await _postRepository.GetTotalCountAsync(includeUnpublished: true);
        var publishedCount = await _postRepository.GetTotalCountAsync(isPublished: true);
        var draftCount = totalCount - publishedCount;
        
        return new
        {
            TotalPosts = totalCount,
            PublishedPosts = publishedCount,
            DraftPosts = draftCount,
            TotalViews = 0, // This would require a separate tracking system
            PostsThisMonth = totalCount // Simplified for now
        };
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
}