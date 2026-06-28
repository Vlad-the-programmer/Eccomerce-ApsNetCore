using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceRestApi.Helpers.Cart
{
    public class ShoppingCart
    {
        private readonly AppDbContext _context;
        private readonly ISession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _cartId;
        private readonly string _userId;
        private readonly bool _isAuthenticated;

        public ShoppingCart(
            AppDbContext context,
            ISession session,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _session = session;
            _httpContextAccessor = httpContextAccessor;

            var user = _httpContextAccessor.HttpContext?.User;
            _isAuthenticated = user?.Identity?.IsAuthenticated ?? false;
            _userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _cartId = GetOrCreateCartId();

            Debug.WriteLine($"CartId: {_cartId}, UserId: {_userId}, IsAuthenticated: {_isAuthenticated}");
        }

        private string GetOrCreateCartId()
        {
            // For authenticated users, use their user ID as the cart identifier
            if (_isAuthenticated && !string.IsNullOrEmpty(_userId))
            {
                // Check if user has an existing cart
                var existingCart = _context.ShoppingCartItems
                    .FirstOrDefault(c => c.UserId == _userId && c.IsActive);

                if (existingCart != null)
                {
                    return existingCart.ShoppingCartId;
                }

                // Create a new cart ID for the user
                var cartId = $"USER_{_userId}_{Guid.NewGuid()}";
                _session.SetString("CartId", cartId);
                return cartId;
            }

            // For guest users, use session ID
            var sessionCartId = _session.GetString("CartId");
            if (!string.IsNullOrEmpty(sessionCartId))
            {
                return sessionCartId;
            }

            // Create new guest cart
            var newCartId = $"GUEST_{Guid.NewGuid()}";
            _session.SetString("CartId", newCartId);
            return newCartId;
        }

        public string GetCartId() => _cartId;
        public string GetUserId() => _userId;
        public bool IsAuthenticated() => _isAuthenticated;

        public async Task AddToCartHandler(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return;

            // Check if item is already in cart
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(c =>
                    c.ShoppingCartId == _cartId &&
                    c.ProductId == productId &&
                    c.IsActive);

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    ProductId = productId,
                    ShoppingCartId = _cartId,
                    UserId = _userId, // Store user ID if authenticated
                    Product = product,
                    Amount = 1,
                    DateCreated = DateTime.Now,
                    IsActive = true
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else if (product.Stock > cartItem.Amount)
            {
                cartItem.Amount++;
                _context.Entry(cartItem).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            // Update session expiry for guest users
            if (!_isAuthenticated)
            {
                _session.SetString("CartId", _cartId);
                _session.SetInt32("CartLastActivity", (int)DateTime.UtcNow.Ticks);
            }
        }

        public async Task DeleteFromCartHandler(int productId)
        {
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(item =>
                    item.ShoppingCartId == _cartId &&
                    item.ProductId == productId &&
                    item.IsActive);

            if (cartItem == null) return;

            if (cartItem.Amount <= 1)
            {
                await DeleteCartItem(cartItem.Id);
            }
            else
            {
                cartItem.Amount--;
                _context.Entry(cartItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ShoppingCartItemDTO>> GetCartItems()
        {
            var query = await _context.ShoppingCartItems
                .Include(item => item.Product)
                .Where(item => item.ShoppingCartId == _cartId && item.IsActive)
                .ToListAsync();

            // For authenticated users, also check by user ID
            if (_isAuthenticated && !string.IsNullOrEmpty(_userId))
            {
                query = query.Where(item => item.UserId == _userId || item.UserId == null).ToList();
            }

            return query
                .Select(item => new ShoppingCartItemDTO
                {
                    Id = item.Id,
                    Amount = item.Amount,
                    ShoppingCartId = item.ShoppingCartId,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ProductPrice = item.Product.Price,
                    DateCreated = item.DateCreated
                }).ToList();
        }

        public async Task<decimal> GetTotal()
        {
            var items = _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == _cartId && item.IsActive)
                .Select(item => (decimal?)item.Amount * item.Product.Price);

            return await items.SumAsync() ?? decimal.Zero;
        }

        public async Task DeleteCartItem(int id)
        {
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(item =>
                    item.ShoppingCartId == _cartId &&
                    item.Id == id &&
                    item.IsActive);

            if (cartItem == null) return;

            cartItem.IsActive = false;
            cartItem.DateDeleted = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task<Order> ConvertToOrder(Order order, List<int> cartItemsIds)
        {
            var orderItems = await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == _cartId
                               && item.IsActive
                               && cartItemsIds.Contains(item.ProductId))
                .Include(item => item.Product)
                .ToListAsync();

            if (!orderItems.Any())
            {
                throw new Exception("No items selected for the order.");
            }

            order.OrderItems = orderItems
                .Select(cartItem => OrderItem.CartItemToOrderItem(cartItem, order.Id))
                .ToList();

            order.TotalAmount = orderItems.Sum(item => item.Product.Price * item.Amount);

            foreach (var item in orderItems)
            {
                var orderItem = OrderItem.CartItemToOrderItem(item, order.Id);
                await _context.OrderItems.AddAsync(orderItem);
                await DeleteCartItem(item.Id);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task ClearCart(List<int>? SelectedItemsIds)
        {
            var cartItems = await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == _cartId && item.IsActive && SelectedItemsIds.Contains(item.ProductId))
                .ToListAsync();

            foreach (var item in cartItems)
            {
                item.IsActive = false;
                item.DateDeleted = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        // Method to merge guest cart into user cart on login
        public async Task MergeCartWithUser(string userId, string guestCartId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(guestCartId))
                return;

            // Get guest cart items
            var guestItems = await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == guestCartId && item.IsActive)
                .ToListAsync();

            if (!guestItems.Any())
            {
                // No items to merge, just update the cart ID
                await UpdateCartIdForUser(userId);
                return;
            }

            // Get or create user's cart
            var userCartId = await GetOrCreateUserCartId(userId);

            // Merge items
            foreach (var guestItem in guestItems)
            {
                // Check if product already exists in user's cart
                var existingItem = await _context.ShoppingCartItems
                    .FirstOrDefaultAsync(item =>
                        item.ShoppingCartId == userCartId &&
                        item.ProductId == guestItem.ProductId &&
                        item.IsActive);

                if (existingItem != null)
                {
                    // Combine quantities
                    existingItem.Amount += guestItem.Amount;
                    _context.Entry(existingItem).State = EntityState.Modified;
                }
                else
                {
                    // Move guest item to user's cart
                    guestItem.ShoppingCartId = userCartId;
                    guestItem.UserId = userId;
                    _context.Entry(guestItem).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();

            // Update session with new cart ID
            _session.SetString("CartId", userCartId);

            // Clear guest cart
            await ClearGuestCart(guestCartId);
        }

        private async Task<string> GetOrCreateUserCartId(string userId)
        {
            var existingCart = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

            if (existingCart != null)
            {
                return existingCart.ShoppingCartId;
            }

            var cartId = $"USER_{userId}_{Guid.NewGuid()}";
            return cartId;
        }

        private async Task UpdateCartIdForUser(string userId)
        {
            var cartId = $"USER_{userId}_{Guid.NewGuid()}";
            _session.SetString("CartId", cartId);
        }

        private async Task ClearGuestCart(string guestCartId)
        {
            var guestItems = await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == guestCartId && item.IsActive)
                .ToListAsync();

            foreach (var item in guestItems)
            {
                item.IsActive = false;
                item.DateDeleted = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CleanupAbandonedCarts(int daysOld = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysOld);
            var abandonedCarts = await _context.ShoppingCartItems
                .Where(item =>
                    item.IsActive &&
                    item.DateCreated < cutoffDate &&
                    string.IsNullOrEmpty(item.UserId))
                .Select(item => item.ShoppingCartId)
                .Distinct()
                .ToListAsync();

            foreach (var cartId in abandonedCarts)
            {
                var items = await _context.ShoppingCartItems
                    .Where(item => item.ShoppingCartId == cartId && item.IsActive)
                    .ToListAsync();

                foreach (var item in items)
                {
                    item.IsActive = false;
                    item.DateDeleted = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}