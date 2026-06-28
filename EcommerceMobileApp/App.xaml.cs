using EcommerceMobileApp.Helpers.Session;

namespace EcommerceMobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await SessionService.Instance.LoadFromStorageAsync();
        }
    }
}