using EcommerceMobileApp.AppLogic.VMs;

namespace EcommerceMobileApp.Views;

public partial class ShopCoinHistoryPage : ContentPage
{
    private readonly ShopCoinTransactionHistoryVM _vm = new();

    public ShopCoinHistoryPage()
    {
        InitializeComponent();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}