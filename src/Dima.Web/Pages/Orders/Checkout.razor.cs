using Dima.Core.Handlers;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public class CheckoutPage : ComponentBase
{
    public PatternMask mask = new PatternMask("####-####")
    {
        MaskChars = new[] { new MaskChar('#', @"[0-9a-fA-F]") },
        Placeholder = '_',
        CleanDelimiters = true,
        Transformation = AllUpperCase
    };

    private static char AllUpperCase(char c) => c.ToString().ToUpperInvariant()[0];

    [Parameter]
    public string Product { get; set; } = string.Empty;

    #region Properties

    public bool IsBusy { get; set; } = false;
    public CreateOrderRequest InputModel { get; set; } = new();

    #endregion

    #region Services

    // [Inject]
    // public IOrderHandler Handler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            // var result = await Handler.CreateAsync(InputModel);
            // if (result.IsSuccess)
            // {
            //     Snackbar.Add(result.Message, Severity.Success);
            //     NavigationManager.NavigateTo("/pedidos");
            // }
            // else
            //     Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
}