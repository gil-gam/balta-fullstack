using Dima.Core;
using Dima.Core.Handlers;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using Dima.Core.Responses.Stripe;
using Stripe;
using Stripe.Checkout;

namespace Dima.Api.Handlers;

public class StripeHandler : IStripeHandler
{
    public async Task<Response<string?>> CreateSessionAsync(CreateSessionRequest request)
    {
        var options = new SessionCreateOptions
        {
            CustomerEmail = request.UserId,
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "order", request.OrderNumber }
                }
            },
            PaymentMethodTypes =
            [
                "card"
            ],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "BRL",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = request.ProductTitle,
                            Description = request.ProductDescription
                        },
                        UnitAmount = request.OrderTotal,
                    },
                    Quantity = 1
                }
            ],
            Mode = "payment",
            SuccessUrl = $"{Configuration.FrontendUrl}/pedidos/{request.OrderNumber}/confirmar",
            CancelUrl = $"{Configuration.FrontendUrl}/pedidos/{request.OrderNumber}/cancelar",
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return new Response<string?>(session.Id);
    }

    public async Task<Response<List<StripeTransactionResponse>>> GetTransactionsByOrderNumberAsync(
        GetTransactionByOrderNumberRequest request)
    {
        var options = new ChargeSearchOptions
        {
            Query = $"metadata['order']:'{request.Number}'",
        };
        var service = new ChargeService();
        var result = await service.SearchAsync(options);

        if (result.Data.Count == 0)
            return new Response<List<StripeTransactionResponse>>(null, 404, "Transação não encontrada");

        var data = new List<StripeTransactionResponse>();
        foreach (var item in result.Data)
        {
            data.Add(new StripeTransactionResponse
            {
                Id = item.Id,
                Email = item.BillingDetails.Email,
                Amount = item.Amount,
                AmountCaptured = item.AmountCaptured,
                Status = item.Status,
                Paid = item.Paid,
                Refunded = item.Refunded,
            });
        }

        return new Response<List<StripeTransactionResponse>>(data);
    }
}