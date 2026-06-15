using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface IWishListService : IEntityBaseRepository<Wishlist>
    {
        Task CreateWishListAsync(string userId);
        Task AddWishlistItem(WishListItemDto item, string userId);
        Task RemoveWishlistItemAsync(int wishlistItemId, string userId);
        Task<List<WishListItemDto>> GetWishlistItemsByUserIdAsync(string userId);
        Task<WishListDto> GetWishlistByUserId(string userId);
    }
}
