using EventOn.API.Models.Enums;

namespace EventOn.API.Models;

public class Event
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public Location Location { get; set; } = new();
    public int Capacity { get; set; }
    public int RemainingTickets { get; set; }
    public EventCategory Category { get; set; }
    public List<string> Photos { get; set; } = [];
    public string Description { get; set; } = string.Empty;
    public List<string> Performers { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
    public List<string> UsersWhoLiked { get; set; } = [];
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string OwnerUsername { get; set; } = string.Empty;
    public bool IsRecommended { get; set; }
}