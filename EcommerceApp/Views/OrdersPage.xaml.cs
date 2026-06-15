using EcommerceApp.AppLogic.Dtos;
using EcommerceApp.AppLogic.Services;
using EcommerceApp.Helpers.Session;

namespace EcommerceApp.Views
{
    public partial class OrdersPage : ContentPage
    {
        public OrdersPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Check if user is logged in using the helper method
                if (!SessionService.Instance.IsLoggedIn())
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                int customerId = SessionService.Instance.CurrentUser.Id;

                // Show loading indicator
                IsBusy = true;

                var orders = await OrdersService.GetUserOrders(customerId);
                OrdersList.ItemsSource = orders;

                // Store orders in session
                SessionService.Instance.UserOrders = (List<OrderDto>)orders;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading orders: {ex.Message}");
                await DisplayAlert("Error", "Failed to load orders", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnOrderSelected(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var order = e.CurrentSelection.FirstOrDefault() as OrderDto;
                if (order != null)
                {
                    await Shell.Current.GoToAsync($"orderdetails?code={order.Code}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting order: {ex.Message}");
            }
        }
    }
}