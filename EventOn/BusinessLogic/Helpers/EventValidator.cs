using EventOn.API.Models;

namespace EventOn.BusinessLogic.Helpers;

public static class EventValidator
{
    public static Result ValidateNewCreatedEvent(Event @event, List<byte[]> photos)
    {
        var result = new Result();

        if (string.IsNullOrEmpty(@event.Name))
            result.AddError("Event Name field is empty");

        if (string.IsNullOrEmpty(@event.Location.Name))
            result.AddError("Event Location cannot be empty");

        if (@event.Performers.Count == 0)
            result.AddError("Event needs performers");

        if (photos.Count == 0)
            result.AddError("Event needs at least one photo");

        if (@event.Price >= 50000)
            result.AddError("Event price its too big");

        return result;
    }
}
