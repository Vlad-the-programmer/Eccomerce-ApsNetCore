using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.VMs;

namespace EcommerceMobileApp.Views
{
    public partial class OrdersPage : ContentPage
    {
        public OrdersPage()
        {
            InitializeComponent();
            BindingContext = new OrdersViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var orders = await (BindingContext as OrdersViewModel).LoadOrdersAsync();
            OrdersList.ItemsSource = orders ?? new List<OrderDto>();

        }
    }
}