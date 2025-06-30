namespace EventOn.API.Models.Stripe;

public class PaymentConfirmationRequest
{
    public string EventId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
}