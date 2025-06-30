using EventOn.API.Models.Enums;

namespace EventOn.API.Models;

public class UserPreferences
{
    public string Username { get; set; } = string.Empty;
    public Theme SelectedTheme { get; set; }
    public NotificationPreferences Notifications { get; set; } = new();
}