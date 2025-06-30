namespace EventOn.API.Models.Interactions;

public class FollowPerformer : Interaction
{
    public string PerformerUsername { get; set; } = string.Empty;
    public string PerformerType { get; set; } = string.Empty;
}