using EventOn.API.Models.Enums;

namespace EventOn.BusinessLogic.Models;

public class EventFilterCriteria
{
    public EventCategory? Category { get; set; } = null;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
}