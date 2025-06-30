namespace EventOn.API.Models.Stripe;

public class PaymentRequest
{
    public string Username { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ron";
    public string Description { get; set; } = "Ticket Purchase";
}