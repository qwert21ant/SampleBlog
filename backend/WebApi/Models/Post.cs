using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Post
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Subtitle { get; set; } = string.Empty;
    
    [Required]
    public string Text { get; set; } = string.Empty;
    
    public bool IsPublished { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? PublishedAt { get; set; }
    
    // Foreign key
    public int AuthorId { get; set; }
    
    // Navigation properties
    public User Author { get; set; } = null!;
    public ICollection<Image> Images { get; set; } = new List<Image>();
}