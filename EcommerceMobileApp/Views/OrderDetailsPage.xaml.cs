using EcommerceMobileApp.AppLogic.Dtos;

namespace EcommerceMobileApp.Views
{
    [QueryProperty(nameof(Order), "Order")]
    public partial class OrderDetailsPage : ContentPage
    {
        public OrderDto Order
        {
            set
            {
                BindingContext = value;
            }
        }

        public OrderDetailsPage()
        {
            InitializeComponent();
        }
    }

}