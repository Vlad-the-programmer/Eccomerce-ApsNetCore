using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface IShopCoinSettingsService : IEntityBaseRepository<ShopCoinSettings>
    {
        Task<ShopCoinSettingsDto> GetShopCoinSettingsAsync();
        Task UpdateShopCoinSettingsAsync(ShopCoinSettingsDto settings);
        Task CreateShopCoinSettingsAsync(ShopCoinSettingsDto settings);
    }
}
