using EcommerceMobileApp.AppLogic.Services;
using EcommerceMobileApp.Helpers.Session;
using System.Windows.Input;

namespace EcommerceMobileApp.AppLogic.VMs;

public class LoginViewModel : BaseViewModel
{
    private string _email;
    private string _password;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await Login());
    }

    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please enter email and password", "OK");
            return;
        }

        IsBusy = true;

        var loginModel = new LoginRequest
        {
            Email = Email,
            Password = Password
        };

        System.Diagnostics.Debug.WriteLine($"Attempting login for: {Email}");

        bool success = await ApiService.LoginAsync(loginModel);

        System.Diagnostics.Debug.WriteLine($"Login success: {success}");
        System.Diagnostics.Debug.WriteLine($"CurrentUser after login: {SessionService.Instance.CurrentUser?.UserId ?? ""}");
        System.Diagnostics.Debug.WriteLine($"IsLoggedIn: {SessionService.Instance.IsLoggedIn()}");

        IsBusy = false;

        if (!success)
        {
            await Application.Current.MainPage.DisplayAlert("Login Failed", "Invalid email or password.", "OK");
            return;
        }

        System.Diagnostics.Debug.WriteLine("Navigating to OrdersPage");

        await ExecuteLoadItems();

        await Task.Delay(100);
        await Shell.Current.GoToAsync("//OrdersPage");
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}