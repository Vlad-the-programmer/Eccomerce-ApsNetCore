using EcommerceMobileApp.Helpers.Session;
using EcommerceMobileApp.Views;

namespace EcommerceMobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AccountDetails), typeof(AccountDetails));
            Routing.RegisterRoute(nameof(OrdersPage), typeof(OrdersPage));
            Routing.RegisterRoute(nameof(OrderDetailsPage), typeof(OrderDetailsPage));
            Routing.RegisterRoute(nameof(RefundsPage), typeof(RefundsPage));
            Routing.RegisterRoute(nameof(RefundDetailsPage), typeof(RefundDetailsPage));
            Routing.RegisterRoute(nameof(ShopCoinHistoryPage), typeof(ShopCoinHistoryPage));

            SessionService.Instance.AuthChanged += UpdateAuthUI;

            UpdateAuthUI();
        }

        public void UpdateAuthUI()
        {
            var isLoggedIn = SessionService.Instance.IsLoggedIn();

            LogoutItem.IsVisible = isLoggedIn;
            OnPropertyChanged(nameof(LogoutItem));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Target.Location.OriginalString.Contains("Logout"))
            {
                args.Cancel();

                Task.Run(async () =>
                {
                    await SessionService.Instance.LogoutAsync();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        (App.Current.MainPage as AppShell)?.UpdateAuthUI();

                        Shell.Current.GoToAsync("//LoginPage");
                    });
                });
            }
        }
    }
}
