namespace EventOn.API.Models;

public class Location
{
    public string Name { get; set; } = string.Empty;
    public string PlaceId { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}