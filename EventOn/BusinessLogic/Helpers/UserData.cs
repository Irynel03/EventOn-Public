using EventOn.API.Models;
using EventOn.API.Models.Enums;

namespace EventOn.BusinessLogic.Helpers;

public sealed class UserData
{
    private static readonly Lazy<UserData> instance = new(() => new UserData());

    private UserData() { }

    public static UserData Instance => instance.Value;

    public string Username { get; set; } = string.Empty;
    public TypeOfUser UserType { get; set; }
    public decimal Balance { get; set; }
    public List<string> FollowedPerformers { get; set; } = [];
    public bool HasActiveCreatedEvents { get; set; }
    public bool HasCard { get; set; }
    public UserPreferences Preferences { get; set; } = new();

    public void SetNewUser(User user, bool hasActiveCreatedEvents = false)
    {
        Username = user.Username;
        HasActiveCreatedEvents = hasActiveCreatedEvents;
        HasCard = !string.IsNullOrWhiteSpace(user.PaymentMethodId);
        Balance = user.Balance;
        FollowedPerformers = user.FollowedUsers;
        UserType = user.UserType;
    }

    public void SetNewUser(User user, UserPreferences userPreferences, bool hasActiveCreatedEvents = false)
    {
        Username = user.Username;
        HasActiveCreatedEvents = hasActiveCreatedEvents;
        HasCard = !string.IsNullOrWhiteSpace(user.PaymentMethodId);
        Balance = user.Balance;
        FollowedPerformers = user.FollowedUsers;
        Preferences = userPreferences;
        UserType = user.UserType;
    }

    public void Reset()
    {
        Username = string.Empty;
        HasActiveCreatedEvents = false;
    }
}