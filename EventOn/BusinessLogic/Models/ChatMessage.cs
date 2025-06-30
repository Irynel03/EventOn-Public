namespace EventOn.BusinessLogic.Models;

public class ChatMessage
{
    public string Sender { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? EventId { get; set; }
}
