using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface IWishListService : IEntityBaseRepository<Wishlist>
    {
        Task<WishListDto> CreateWishListAsync(string userId);
        Task AddWishlistItem(int productId, string userId);
        Task RemoveWishlistItemAsync(int wishlistItemId, string userId);
        Task<List<WishListItemDto>> GetWishlistItemsByUserIdAsync(string userId);
        Task<WishListDto> GetWishlistByUserId(string userId);
        Task TransferWishlistToCartAsync(string userId);
        Task ClearWishlistAsync(string userId);
    }
}
