using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.Services;
using EcommerceMobileApp.AppLogic.VMs;
using System.Windows.Input;

public class AccountDetailsViewModel : BaseViewModel
{
    private UserProfileDto _user;

    public UserProfileDto User
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    public ICommand OpenCoinHistoryCommand { get; }

    public AccountDetailsViewModel()
    {
        OpenCoinHistoryCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("//ShopCoinHistoryPage");
        });
    }

    public async Task<UserProfileDto> LoadUserAsync()
    {
        var profile = await AccountService.GetProfile();
        User = profile;

        return profile;
    }
}