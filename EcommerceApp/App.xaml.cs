using EcommerceApp.Helpers.Session;

namespace EcommerceApp
{
    public partial class App : Application
    {
        public App()
        {
            Task.Run(async () => await SessionService.Instance.LoadFromStorage());

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await SessionService.Instance.LoadFromStorage();
        }
    }
}