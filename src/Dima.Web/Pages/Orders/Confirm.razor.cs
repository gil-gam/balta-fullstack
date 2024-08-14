using Dima.Core.Handlers;
using Microsoft.AspNetCore.Components;

namespace Dima.Web.Pages.Orders;

public partial class ConfirmOrderPaymentPage : ComponentBase
{
    #region Parameters

    [Parameter] public string Number { get; set; } = string.Empty;

    #endregion

    #region Services

    [Inject] public IOrderHandler OrderHandler { get; set; } = null!;

    #endregion

    #region Overrides

    protected override Task OnInitializedAsync()
    {
        
    }

    #endregion
}