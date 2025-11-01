using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DTOs;

public class ImageUploadRequest
{
    [Required]
    public IFormFile Image { get; set; } = null!;
    
    [MaxLength(500)]
    public string? AltText { get; set; }
}

public class CreatePostRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Subtitle { get; set; } = string.Empty;
    
    [Required]
    public string Text { get; set; } = string.Empty;
    
    public bool IsPublished { get; set; }
}

public class UpdatePostRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Subtitle { get; set; } = string.Empty;
    
    [Required]
    public string Text { get; set; } = string.Empty;
    
    public bool IsPublished { get; set; }
}

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public UserDto Author { get; set; } = null!;
}

public class PostPublicDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
}

public class PostSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
}

public class ImageDetailsDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? AltText { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class PostStatsDto
{
    public int TotalPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int DraftPosts { get; set; }
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}