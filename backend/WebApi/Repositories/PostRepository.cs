using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories;

public class PostRepository : IPostRepository
{
    private readonly BlogDbContext _context;

    public PostRepository(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(int id, bool includeUnpublished = false)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .AsQueryable();
        
        if (!includeUnpublished)
        {
            query = query.Where(p => p.IsPublished);
        }
        
        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Post>> GetAllAsync(int page, int pageSize, bool? isPublished = null)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .AsQueryable();
        
        if (isPublished.HasValue)
        {
            query = query.Where(p => p.IsPublished == isPublished.Value);
        }
        
        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(bool? isPublished = null)
    {
        var query = _context.Posts.AsQueryable();
        
        if (isPublished.HasValue)
        {
            query = query.Where(p => p.IsPublished == isPublished.Value);
        }
        
        return await query.CountAsync();
    }

    public async Task<IEnumerable<Post>> GetRecentAsync(int count)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByAuthorAsync(int authorId, int page, int pageSize, bool? isPublished = null)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Where(p => p.AuthorId == authorId);

        if (isPublished.HasValue)
        {
            query = query.Where(p => p.IsPublished == isPublished.Value);
        }

        return await query
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetByAuthorCountAsync(int authorId, bool? isPublished)
    {
        var query = _context.Posts.Where(p => p.AuthorId == authorId);
        
        if (isPublished.HasValue)
        {
            query = query.Where(p => p.IsPublished == isPublished.Value);
        }
        
        return await query.CountAsync();
    }

    public async Task<Post> CreateAsync(Post post)
    {
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;
        
        if (post.IsPublished && post.PublishedAt == null)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        
        // Load the author information
        await _context.Entry(post)
            .Reference(p => p.Author)
            .LoadAsync();
        
        return post;
    }

    public async Task<Post> UpdateAsync(Post post)
    {
        var existingPost = await _context.Posts.FindAsync(post.Id);
        if (existingPost == null)
            throw new KeyNotFoundException($"Post with ID {post.Id} not found");
        
        existingPost.Title = post.Title;
        existingPost.Subtitle = post.Subtitle;
        existingPost.Text = post.Text;
        existingPost.UpdatedAt = DateTime.UtcNow;
        
        // Handle publishing status changes
        if (post.IsPublished != existingPost.IsPublished)
        {
            existingPost.IsPublished = post.IsPublished;
            if (post.IsPublished && existingPost.PublishedAt == null)
            {
                existingPost.PublishedAt = DateTime.UtcNow;
            }
            else if (!post.IsPublished)
            {
                existingPost.PublishedAt = null;
            }
        }
        
        await _context.SaveChangesAsync();
        
        // Load the author information
        await _context.Entry(existingPost)
            .Reference(p => p.Author)
            .LoadAsync();
        
        return existingPost;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }
}