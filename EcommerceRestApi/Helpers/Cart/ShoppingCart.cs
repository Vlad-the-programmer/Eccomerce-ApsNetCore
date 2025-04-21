using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
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
            //check if item is already in cart!
            var cartItem =
                (
                from cartitem in _context.ShoppingCartItems
                where cartitem.ShoppingCartId == this.IdCartSession && cartitem.ProductId == productId
                select cartitem
                ).FirstOrDefault();

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    ProductId = productId,
                    ShoppingCartId = this.IdCartSession,
                    Product = await _context.Products.FindAsync(productId) ?? new(),
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

        public async Task DeleteFromCartHandler(int idCartItem)
        {
            var cartItem = await _context.ShoppingCartItems.FindAsync(idCartItem);
            if (cartItem != null)
            {
                _context.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<ShoppingCartItem>> GetCartItems()
            => await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == IdCartSession)
                .Include(item => item.Product)
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

        public async Task<Order> ConvertToOrder(Order order)
        {
            var orderItems = await GetCartItems();
            order.TotalAmount = await GetTotal();

            foreach (var item in orderItems)
            {
                if (item == null)
                {
                    continue;
                }
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Amount,
                    UnitPrice = item.Product.Price,
                    DateCreated = DateTime.Now,
                    IsActive = item.IsActive,
                    OrderId = order.Id,
                };
                await _context.OrderItems.AddAsync(orderItem);
            }
            await _context.SaveChangesAsync();
            return order;
        }


    }
}
