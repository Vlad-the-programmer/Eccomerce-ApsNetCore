using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.Services;
using EcommerceMobileApp.Helpers.Session;
using EcommerceMobileApp.Views;
using System.Windows.Input;

namespace EcommerceMobileApp.AppLogic.VMs
{
    public class RefundsViewModel : BaseViewModel
    {
        public ICommand ViewRefundCommand { get; }

        public RefundsViewModel()
        {
            ViewRefundCommand = new Command<RefundDto>(OnViewRefund);
        }

        private async void OnViewRefund(RefundDto refund)
        {
            if (refund == null) return;

            await Shell.Current.GoToAsync(nameof(RefundDetailsPage), new Dictionary<string, object>
            {
                { "Refund", refund }
            });
        }

        public async Task<IEnumerable<RefundDto>?> LoadRefundsAsync()
        {
            try
            {
                if (!await SessionService.Instance.IsLoggedInAsync())
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return null;
                }


                var refunds = await RefundService.GetUserRefunds(SessionService.Instance.CurrentUser.CustomerId);

                foreach (var refund in refunds)
                {
                    var history = refund.RefundStatusHistory?.ToList();

                    if (history == null || history.Count == 0)
                        continue;

                    for (int i = 0; i < history.Count; i++)
                    {
                        history[i].IsCurrent = false;
                    }

                    history[^1].IsCurrent = true;

                    refund.RefundStatusHistory = history;
                }

                return refunds;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading refunds: {ex.Message}");
                return null;
            }
        }
    }
}
