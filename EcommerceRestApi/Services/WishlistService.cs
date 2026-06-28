using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;
using Microsoft.EntityFrameworkCore;


namespace EcommerceRestApi.Services
{
    public class WishlistService : EntityBaseRepository<Wishlist>, IWishListService
    {
        private readonly AppDbContext _context;
        private readonly ShoppingCart _cart;

        public WishlistService(AppDbContext context, ShoppingCart cart) : base(context)
        {
            _context = context;
            _cart = cart;
        }

        // 🔹 CREATE DEFAULT WISHLIST
        public async Task<WishListDto> CreateWishListAsync(string userId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                throw new Exception("Customer not found");

            var existing = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.CustomerId == customer.Id && w.IsDefault);

            if (existing != null)
                return await GetWishlistByUserId(userId);

            var wishlist = new Wishlist
            {
                CustomerId = customer.Id,
                Name = "Default Wishlist",
                IsDefault = true,
                IsActive = true
            };

            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();

            return await GetWishlistByUserId(userId);
        }

        // 🔹 ADD ITEM
        public async Task AddWishlistItem(int productId, string userId)
        {
            await CreateWishListAsync(userId);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                throw new Exception("Customer not found");

            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.CustomerId == customer.Id && w.IsDefault);

            if (wishlist == null)
                throw new Exception("Wishlist not found");

            var exists = await _context.WishlistItems
                .AnyAsync(wi => wi.WishlistId == wishlist.Id && wi.ProductId == productId);

            if (exists)
                return;

            var wishlistItem = new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = productId,
                IsActive = true
            };

            await _context.WishlistItems.AddAsync(wishlistItem);
            await _context.SaveChangesAsync();
        }

        // 🔹 GET FULL WISHLIST
        public async Task<WishListDto> GetWishlistByUserId(string userId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                throw new Exception("Customer not found");

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.CustomerId == customer.Id && w.IsDefault);

            if (wishlist == null)
                throw new Exception("Wishlist not found");

            return new WishListDto
            {
                CustomerId = customer.Id,
                Name = wishlist.Name,
                IsDefault = wishlist.IsDefault,
                WishlistItems = wishlist.WishlistItems.Where(wi => wi.IsActive)
                .Select(wi => new WishListItemDto
                {
                    Id = wi.Id,
                    ProductId = wi.ProductId,
                    WishlistId = wi.WishlistId,
                    ProductName = wi.Product.Name,
                    ProductBrand = wi.Product.Brand,
                    Price = wi.Product.Price
                }).ToList()
            };
        }

        // 🔹 GET ITEMS ONLY
        public async Task<List<WishListItemDto>> GetWishlistItemsByUserIdAsync(string userId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                throw new Exception("Customer not found");

            var wishlist = await _context.Wishlists
                .Where(w => w.IsActive)
                .FirstOrDefaultAsync(w => w.CustomerId == customer.Id && w.IsDefault);

            if (wishlist == null)
                return new List<WishListItemDto>();

            return await _context.WishlistItems
                .Where(wi => wi.WishlistId == wishlist.Id && wi.IsActive)
                .Select(wi => new WishListItemDto
                {
                    Id = wi.Id,
                    ProductId = wi.ProductId,
                    WishlistId = wi.WishlistId,
                    ProductName = wi.Product.Name,
                    ProductBrand = wi.Product.Brand,
                    Price = wi.Product.Price
                })
                .ToListAsync();
        }

        // 🔹 REMOVE ITEM
        public async Task RemoveWishlistItemAsync(int wishlistItemId, string userId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                throw new Exception("Customer not found");

            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.CustomerId == customer.Id && w.IsDefault);

            if (wishlist == null)
                throw new Exception("Wishlist not found");

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(wi => wi.Id == wishlistItemId && wi.WishlistId == wishlist.Id);

            if (item == null)
                throw new Exception("Item not found");

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task TransferWishlistToCartAsync(string userId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                throw new Exception("Customer not found");

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.CustomerId == customer.Id && w.IsDefault);

            if (wishlist == null || !wishlist.WishlistItems.Any())
                throw new Exception("Wishlist is empty");

            foreach (var item in wishlist.WishlistItems)
            {
                await _cart.AddToCartHandler(item.ProductId);

                item.IsActive = false;
                item.DateDeleted = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearWishlistAsync(string userId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                throw new Exception("Customer not found");

            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.CustomerId == customer.Id && w.IsDefault);

            if (wishlist == null)
                throw new Exception("Wishlist not found");

            var items = await _context.WishlistItems
                .Where(wi => wi.WishlistId == wishlist.Id && wi.IsActive)
                .ToListAsync();

            if (!items.Any())
                return;

            foreach (var item in items)
            {
                item.IsActive = false;
                item.DateDeleted = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}