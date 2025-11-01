using WebApi.Models;
using WebApi.Models.DTOs;

namespace WebApi.Services;

public static class MappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username
        };
    }

    public static PostDto ToDto(this Post post)
    {
        var firstImage = post.Images?.FirstOrDefault();
        
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Subtitle = post.Subtitle,
            Text = post.Text,
            MainImageUrl = firstImage != null 
                ? $"/api/images/{firstImage.Id}" 
                : null,
            IsPublished = post.IsPublished,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            PublishedAt = post.PublishedAt,
            Author = post.Author.ToDto()
        };
    }

    public static PostSummaryDto ToSummaryDto(this Post post)
    {
        var firstImage = post.Images?.FirstOrDefault();
        
        return new PostSummaryDto
        {
            Id = post.Id,
            Title = post.Title,
            Subtitle = post.Subtitle,
            MainImageUrl = firstImage != null 
                ? $"/api/images/{firstImage.Id}" 
                : null,
            CreatedAt = post.CreatedAt,
            PublishedAt = post.PublishedAt,
            Author = post.Author.ToDto()
        };
    }

    public static Post ToEntity(this CreatePostRequest request, int authorId)
    {
        return new Post
        {
            Title = request.Title,
            Subtitle = request.Subtitle,
            Text = request.Text,
            IsPublished = request.IsPublished,
            AuthorId = authorId
        };
    }

    public static void UpdateFromDto(this Post post, UpdatePostRequest request)
    {
        post.Title = request.Title;
        post.Subtitle = request.Subtitle;
        post.Text = request.Text;
        post.IsPublished = request.IsPublished;
    }

    public static User ToEntity(this RegisterRequest request, string passwordHash)
    {
        return new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            Username = request.Username
        };
    }
}