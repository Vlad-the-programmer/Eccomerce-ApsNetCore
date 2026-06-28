using EcommerceApp.Helpers.Session;
using EcommerceMobileApp.Views;

namespace EcommerceApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("orderdetails", typeof(OrderDetailsPage));

            // Set initial page based on login status - use the x:Name from XAML
            if (SessionService.Instance.IsLoggedIn())
            {
                // Assuming you have set x:Name="OrdersShellContent" in XAML
                CurrentItem = OrdersShellContent;
            }
            else
            {
                CurrentItem = LoginShellContent;
            }
        }

        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            var target = args.Target.Location.OriginalString;

            if (target.Contains("OrdersPage") || target.Contains("OrderDetailsPage"))
            {
                if (!SessionService.Instance.IsLoggedIn())
                {
                    args.Cancel();
                    await GoToAsync("//LoginPage");
                }
            }
        }
    }
}