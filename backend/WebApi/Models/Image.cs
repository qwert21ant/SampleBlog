using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Image
{
    public int Id { get; set; }
    
    [Required]
    public byte[] Data { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string FileName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? AltText { get; set; }
    
    public long Size { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign key for post
    public int PostId { get; set; }
    
    // Navigation property
    public Post Post { get; set; } = null!;
}