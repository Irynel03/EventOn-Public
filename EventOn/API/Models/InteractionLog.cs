using EventOn.API.Models.Interactions;

namespace EventOn.API.Models;

public class InteractionLog
{
    public List<EventInteraction> EventInteractions { get; set; } = [];
    public List<FollowPerformer> PerformersInterations { get; set; } = [];
}