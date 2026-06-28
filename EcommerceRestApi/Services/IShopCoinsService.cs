using EcommerceRestApi.Models.Dtos;

namespace EcommerceRestApi.Services
{
    public interface IShopCoinsService
    {
        Task<IList<ShopCoinTransactionHistoryDto>> GetAllCoinsHistory(string userId);
        Task RewardCoinsForOrder(int orderId);
        Task SpendCoinsForOrder(int orderId);
        Task RefundCoinsForOrder(int orderId);
        Task ChangeCustomerPoints(int customerId);
        Task<int> GetCustomerBalance(string userId);
        Task<int> ConvertMoneyToCoins(decimal amount);
        Task<decimal> ConvertFromCoinsToMoney(int coins);
        Task<int> CalculateMaxCoinsToSpend(int customerId, decimal totalAmount);

    }
}
