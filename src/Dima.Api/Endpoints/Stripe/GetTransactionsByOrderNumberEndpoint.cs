using System.Security.Claims;
using Azure;
using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Requests.Stripe;

namespace Dima.Api.Endpoints.Stripe;

public class GetTransactionsByOrderNumberEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{number}/transactions", HandleAsync)
            .Produces<Response<dynamic>>();

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        IStripeHandler handler,
        string number)
    {
        var request = new GetTransactionByOrderNumberRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            Number = number
        };

        var result = await handler.GetTransactionsByOrderNumberAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}