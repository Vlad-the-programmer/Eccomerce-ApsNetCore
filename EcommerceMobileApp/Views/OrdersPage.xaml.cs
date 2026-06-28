using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.Services;
using EcommerceMobileApp.Helpers.Session;

namespace EcommerceMobileApp.Views
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
                if (!await SessionService.Instance.IsLoggedInAsync())
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }


                IsBusy = true;

                var orders = await OrdersService.GetUserOrders(SessionService.Instance.CurrentUser.CustomerId);
                OrdersList.ItemsSource = orders;

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