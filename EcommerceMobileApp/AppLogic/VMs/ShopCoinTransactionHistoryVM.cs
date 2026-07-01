using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.Services;
using EcommerceMobileApp.AppLogic.VMs;
using System.Collections.ObjectModel;

public class ShopCoinTransactionHistoryVM : BaseViewModel
{
    public ObservableCollection<ShopCoinTransactionHistoryDto> Transactions { get; set; }
        = new ObservableCollection<ShopCoinTransactionHistoryDto>();

    private bool isLoading;
    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;

            var data = await AccountService.GetShopCoinTransactionHistory();

            Transactions.Clear();

            foreach (var item in data)
                Transactions.Add(item);
        }
        finally
        {
            IsLoading = false;
        }
    }
}