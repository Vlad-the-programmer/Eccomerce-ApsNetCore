using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.Services;
using EcommerceMobileApp.Helpers.Session;
using EcommerceMobileApp.Views;
using System.Windows.Input;

namespace EcommerceMobileApp.AppLogic.VMs
{
    public class OrdersViewModel : BaseViewModel
    {
        public ICommand ViewOrderCommand { get; }

        public OrdersViewModel()
        {
            ViewOrderCommand = new Command<OrderDto>(OnViewOrder);
        }

        private async void OnViewOrder(OrderDto order)
        {
            if (order == null) return;

            await Shell.Current.GoToAsync(nameof(OrderDetailsPage), new Dictionary<string, object>
            {
                { "Order", order }
            });
        }

        public async Task<IEnumerable<OrderDto>?> LoadOrdersAsync()
        {
            try
            {
                if (!await SessionService.Instance.IsLoggedInAsync())
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return null;
                }


                var orders = await OrdersService.GetUserOrders(SessionService.Instance.CurrentUser.CustomerId);

                SessionService.Instance.UserOrders = orders?.ToList() ?? new List<OrderDto>();

                return orders;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading orders: {ex.Message}");
                return null;
            }
        }
    }
}
