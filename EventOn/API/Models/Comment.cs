using System.ComponentModel.DataAnnotations;

namespace EventOn.API.Models;

public class Comment
{
    [Required]
    public string Id { get; set; } = string.Empty;
    [Required]
    public string Content { get; set; } = string.Empty;
    [Required]
    public string UserName { get; set; } = string.Empty;
    public List<string> LikedByUsers { get; set; } = [];
    public DateTime DatePosted { get; set; } = DateTime.Today;
}