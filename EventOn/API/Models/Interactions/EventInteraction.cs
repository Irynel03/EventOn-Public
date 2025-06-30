using EventOn.API.Models.Enums;
namespace EventOn.API.Models.Interactions;

public class EventInteraction : Interaction
{
    public EventInteractionType Type { get; set; }
    public string EventId { get; set; }
    public string EventOwnerName { get; set; }
    public EventCategory EventCategory { get; set; }
    public List<string> Performers { get; set; }
    public string Location { get; set; }
}