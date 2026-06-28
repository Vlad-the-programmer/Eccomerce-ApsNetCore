using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.Services;
using EcommerceMobileApp.Helpers.Session;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace EcommerceMobileApp.AppLogic.VMs
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private CurrentUserViewModel _currentUser;
        public CurrentUserViewModel CurrentUser
        {
            get => _currentUser;
            set
            {
                SetProperty(ref _currentUser, value);
                OnPropertyChanged(nameof(IsUserLoggedIn)); // Notify UI when user logs in/out
                OnPropertyChanged(nameof(IsUserLoggedOut));
            }
        }

        public bool IsUserLoggedIn => CurrentUser != null && CurrentUser.IsAuthenticated;
        public bool IsUserLoggedOut => !IsUserLoggedIn;

        private IList<OrderDto> _usersOrders;
        public IList<OrderDto> UsersOrders
        {
            get => _usersOrders;
            set
            {
                SetProperty(ref _usersOrders, value);
            }
        }

        public BaseViewModel()
        {
            CurrentUser = SessionService.Instance.CurrentUser;
            //ExecuteLoadItems().GetAwaiter().GetResult();
            Debug.WriteLine("LoggedIn: " + IsUserLoggedIn);
        }

        public async Task ExecuteLoadItems()
        {
            IsBusy = true;

            if (CurrentUser == null)
            {
                UsersOrders = new List<OrderDto>();
                return;
            }

            try
            {
                UsersOrders = (IList<OrderDto>)await OrdersService.GetUserOrders(CurrentUser.CustomerId);
            }
            catch (NullReferenceException e)
            {
                UsersOrders = new List<OrderDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string PageTitle
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
