namespace EcommerceMobileApp.AppLogic.Services
{
    public class AccountService
    {
        public static async Task<Dtos.UserProfileDto> GetProfile()
        {
            var response = await ApiService.GetAsync<Dtos.UserProfileDto>(
                $"api/account/user-profile/");

            return response;
        }

        public static async Task<List<Dtos.ShopCoinTransactionHistoryDto>> GetShopCoinTransactionHistory()
        {
            var response = await ApiService.GetAsync<List<Dtos.ShopCoinTransactionHistoryDto>>(
                $"api/shopcoins/history/");

            return response;
        }
    }
}
