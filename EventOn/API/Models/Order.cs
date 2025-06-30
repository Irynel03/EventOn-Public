namespace EventOn.API.Models;

public class Order
{
    public string Id { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string QrContent { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double Price { get; set; }
    public bool Scanned { get; set; }
}