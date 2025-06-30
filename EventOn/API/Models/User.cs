using EventOn.API.Models.Enums;

namespace EventOn.API.Models;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PaymentMethodId { get; set; } = string.Empty;
    public List<string> OneSignalPlayerIds { get; set; } = [];
    public decimal Balance { get; set; }
    public TypeOfUser UserType { get; set; }
    public List<string> Followers { get; set; } = [];
    public List<string> FollowedUsers { get; set; } = [];
    public List<string> FollowedEvents { get; set; } = [];
    public InteractionLog Interactions { get; set; } = new();
}