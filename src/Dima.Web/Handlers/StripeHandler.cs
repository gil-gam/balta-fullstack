using System.Net.Http.Json;
using Dima.Core.Handlers;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using Dima.Core.Responses.Stripe;

namespace Dima.Web.Handlers;

public class StripeHandler(IHttpClientFactory httpClientFactory) : IStripeHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<Response<string?>> CreateSessionAsync(CreateSessionRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/payments/stripe/session", request);
        return await result.Content.ReadFromJsonAsync<Response<string?>>()
               ?? new Response<string?>(null, 400, "Falha ao criar sessão no Stripe");
    }

    public async Task<Response<List<StripeTransactionResponse>>> GetTransactionsByOrderNumberAsync(
        GetTransactionByOrderNumberRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/payments/stripe/{request.Number}/transactions", request);
        return await result.Content.ReadFromJsonAsync<Response<List<StripeTransactionResponse>>>()
               ?? new Response<List<StripeTransactionResponse>>(null, 400, "Falha ao obter transações do pedido");
    }
}