namespace EcommerceMobileApp.Views;

public partial class AccountDetails : ContentPage
{
    private readonly AccountDetailsViewModel _vm;

    public AccountDetails()
    {
        InitializeComponent();
        _vm = new AccountDetailsViewModel();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _vm.LoadUserAsync();
    }
}