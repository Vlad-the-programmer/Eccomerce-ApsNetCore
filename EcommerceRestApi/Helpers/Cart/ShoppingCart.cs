using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommerceRestApi.Helpers.Cart
{
    public class ShoppingCart
    {
        private readonly AppDbContext _context;
        private readonly ISession _session;
        public string IdCartSession;

        public ShoppingCart(AppDbContext context, ISession session)
        {
            _context = context;
            _session = session;
            IdCartSession = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            Debug.WriteLine($"CartId: {IdCartSession}");
        }

        public ShoppingCart GetShoppingCart()
        {
            _session.SetString("CartId", IdCartSession);
            return new ShoppingCart(_context, _session);
        }

        public async Task AddToCartHandler(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return;
            }

            //check if item is already in cart!
            var cartItem =
                (
                from cartitem in _context.ShoppingCartItems
                where cartitem.ShoppingCartId == this.IdCartSession
                        && cartitem.ProductId == productId
                        && cartitem.IsActive == true
                select cartitem
                ).FirstOrDefault();

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    ProductId = productId,
                    ShoppingCartId = this.IdCartSession,
                    Product = product,
                    Amount = 1,
                    DateCreated = DateTime.Now,
                    IsActive = true
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Amount++;
                _context.Entry(cartItem).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFromCartHandler(int productId)
        {
            var cartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(item =>
                                                    item.ShoppingCartId == IdCartSession
                                                    && item.ProductId == productId);
            if (cartItem != null)
            {
                if (cartItem.Amount <= 1)
                {
                    await DeleteCratItem(cartItem.Id);
                }
                else
                {
                    cartItem.Amount--;
                    _context.Entry(cartItem).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<ShoppingCartItemDTO>> GetCartItems()
            => await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == IdCartSession
                               && item.IsActive)
                .Include(item => item.Product)
                .Select(item => new ShoppingCartItemDTO
                {
                    Id = item.Id,
                    Amount = item.Amount,
                    ShoppingCartId = IdCartSession,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ProductPrice = item.Product.Price,
                    DateCreated = DateTime.Now
                })
                .ToListAsync();


        public async Task<decimal> GetTotal()
        {
            var items =
            (
                from element in _context.ShoppingCartItems
                where element.ShoppingCartId == IdCartSession
                select (decimal?)element.Amount * element.Product.Price
                );

            return await items.SumAsync() ?? decimal.Zero;
        }

        public async Task DeleteCratItem(int id)
        {
            var cartItem = await _context.ShoppingCartItems
                                    .FirstOrDefaultAsync(item =>
                                        item.ShoppingCartId == IdCartSession
                                        && item.Id == id);

            if (cartItem == null) return;

            cartItem.IsActive = false;
            cartItem.DateDeleted = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<Order> ConvertToOrder(Order order)
        {
            var orderItems = await _context.ShoppingCartItems
                                    .Where(item =>
                                            item.ShoppingCartId == IdCartSession
                                            && item.IsActive)
                                    .Include(item => item.Product)
                                    .ToListAsync();
            order.TotalAmount = await GetTotal();
            order.OrderItems = orderItems.Select(cartItem =>
                                    OrderItem.CartItemToOrderItem(cartItem, order.Id))
                                .ToList();
            foreach (var item in orderItems)
            {
                if (item == null)
                {
                    continue;
                }
                var orderItem = OrderItem.CartItemToOrderItem(item, order.Id);
                await _context.OrderItems.AddAsync(orderItem);

                // Delete items from cart after submitting an order
                await DeleteCratItem(item.Id);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task ClearCart()
        {
            var cartItems = await GetCartItems();

            foreach (var cartItem in cartItems)
            {
                await DeleteCratItem(cartItem.Id);
            }
        }

    }
}
