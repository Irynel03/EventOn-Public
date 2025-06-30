namespace EventOn.API.Models;

public class NotificationPreferences
{
    public bool AllDisabled { get; set; }
    public bool FollowedPerformer { get; set; }
    public bool CommentLike { get; set; }
    public bool CommentPost { get; set; }
    public bool NewEvents { get; set; }
}