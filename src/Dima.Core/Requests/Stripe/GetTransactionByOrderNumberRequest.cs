namespace Dima.Core.Requests.Stripe;

public class GetTransactionByOrderNumberRequest : Request
{
    public string Number { get; set; } = string.Empty;
}