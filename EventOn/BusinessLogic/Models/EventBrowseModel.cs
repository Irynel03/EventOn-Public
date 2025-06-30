using EventOn.API.Models;
using EventOn.API.Models.Enums;

namespace EventOn.BusinessLogic.Models;

public class EventBrowseModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public string MainImage { get; set; } = string.Empty;
    public EventCategory Category { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Likes { get; set; }

    public void SetDataFromEvent(Event @event)
    {
        Id = @event.Id;
        Name = @event.Name;
        Likes = @event.UsersWhoLiked.Count;
        Location = @event.Location.Name;
        Category = @event.Category;
        StartDate = @event.StartDate;
        EndDate = @event.EndDate;
        Price = @event.Price;
        MainImage = @event.Photos.Count > 0 ? @event.Photos.First() :
            "https://upload.wikimedia.org/wikipedia/commons/1/14/No_Image_Available.jpg";
    }
}