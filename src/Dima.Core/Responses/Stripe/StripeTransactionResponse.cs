namespace Dima.Core.Responses.Stripe;

public class StripeTransactionResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long Amount { get; set; }
    public long AmountCaptured { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Paid { get; set; }
    public bool Refunded { get; set; }
}