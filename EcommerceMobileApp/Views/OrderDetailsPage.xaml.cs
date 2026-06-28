namespace EcommerceMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailsPage : ContentPage
    {
        public OrderDetailsPage(string code)
        {
            InitializeComponent();
            LoadOrder(code);
        }

        private async void LoadOrder(string code)
        {
            var order = await AppLogic.Services.OrdersService.GetOrder(code);
            BindingContext = order;
        }
    }

}