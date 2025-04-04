using EcommerceRestApi.Models.Context;

namespace EcommerceRestApi.Helpers.Cart
{
    public class ShoppingCart
    {
        public static ShoppingCart GetShoppingCart(IServiceProvider services, ISession session)
        {
            var context = services.GetService<AppDbContext>();

            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);
            return new ShoppingCart();
        }
    }
}
