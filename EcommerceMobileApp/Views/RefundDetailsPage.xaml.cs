using EcommerceMobileApp.AppLogic.Dtos;

namespace EcommerceMobileApp.Views;

[QueryProperty(nameof(Refund), "Refund")]
public partial class RefundDetailsPage : ContentPage
{
    public RefundDto Refund
    {
        set => BindingContext = value;
    }

    public RefundDetailsPage()
    {
        InitializeComponent();
    }
}