using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public class CheckoutPage : ComponentBase
{
    #region Parameters

    [Parameter] public string ProductSlug { get; set; } = string.Empty;

    [SupplyParameterFromQuery(Name = "voucher")]
    public string? VoucherCode { get; set; }

    #endregion

    #region Properties

    public PatternMask Mask = new("####-####")
    {
        MaskChars = [new MaskChar('#', @"[0-9a-fA-F]")],
        Placeholder = '_',
        CleanDelimiters = true,
        Transformation = AllUpperCase
    };

    public bool IsBusy { get; set; } = false;
    public bool IsValid { get; set; } = false;
    public CreateOrderRequest InputModel { get; set; } = new();
    public Product? Product { get; set; }
    public Voucher? Voucher { get; set; }
    public decimal Total { get; set; }

    #endregion

    #region Services

    [Inject] public IProductHandler ProductHandler { get; set; } = null!;
    [Inject] public IOrderHandler OrderHandler { get; set; } = null!;
    [Inject] public IVoucherHandler VoucherHandler { get; set; } = null!;

    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await ProductHandler.GetBySlugAsync(new GetProductBySlugRequest { Slug = ProductSlug });
            if (result.IsSuccess == false)
            {
                Snackbar.Add("Não foi possível obter o produto", Severity.Error);
                IsValid = false;
                return;
            }

            Product = result.Data;
        }
        catch
        {
            Snackbar.Add("Não foi possível obter o produto", Severity.Error);
            IsValid = false;
            return;
        }

        if (Product is null)
        {
            Snackbar.Add("Produto não encontrado", Severity.Error);
            IsValid = false;
            return;
        }

        if (!string.IsNullOrEmpty(VoucherCode))
        {
            try
            {
                var result = await VoucherHandler.GetByNumberAsync(
                    new GetVoucherByNumberRequest
                        { Number = VoucherCode.Replace("-", "") });
                if (!result.IsSuccess)
                {
                    VoucherCode = string.Empty;
                    Snackbar.Add("Não foi possível obter o voucher", Severity.Error);
                }

                if (result.Data is null)
                {
                    VoucherCode = string.Empty;
                    Snackbar.Add("Voucher não encontrado", Severity.Error);
                }

                Voucher = result.Data;
            }
            catch
            {
                VoucherCode = string.Empty;
                Snackbar.Add("Não foi possível obter o voucher", Severity.Error);
            }
        }

        IsValid = true;
        Total = Product.Price - (Voucher?.Amount ?? 0);
    }

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            var request = new CreateOrderRequest
            {
                ProductId = Product!.Id,
                VoucherId = Voucher?.Id ?? null
            };

            var result = await OrderHandler.CreateAsync(request);
            if (result.IsSuccess)
                NavigationManager.NavigateTo($"/pedidos/{result.Data!.Number}");
            else
                Snackbar.Add(result.Message, Severity.Error);
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

    private static char AllUpperCase(char c) => c.ToString().ToUpperInvariant()[0];

    #endregion
}