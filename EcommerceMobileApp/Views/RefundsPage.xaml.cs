using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.VMs;

namespace EcommerceMobileApp.Views;

public partial class RefundsPage : ContentPage
{
    private readonly RefundsViewModel _vm;

    public RefundsPage()
    {
        InitializeComponent();
        _vm = new RefundsViewModel();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var refunds = await _vm.LoadRefundsAsync();
        RefundsList.ItemsSource = refunds != null ? refunds : new List<RefundDto>();
    }
}