using EcommerceRestApi.Models;
using EcommerceWebApp.Models;

namespace EcommerceWebApp.Helpers.Cart
{
    public class ShoppingCart
    {

        public string ShoppingCartId { get; set; }

        public List<ShoppingCartItemVM> ShoppingCartItems { get; set; }

        private readonly ISession _session;

        private ShoppingCart(ISession? session = null)
        {
            _session = session;
        }

        public static ShoppingCart GetShoppingCart(IServiceProvider services, ISession? session)
        {
            if (session == null)
            {
                session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            }
            return new ShoppingCart(session);
        }

        public void AddItemToCart(NewProductViewModel product)
        {

        }

        public void RemoveItemFromCart(NewProductViewModel product)
        {
        }

        public List<ShoppingCartItemVM> GetShoppingCartItems()
        {
            return new List<ShoppingCartItemVM>();
        }

        public double GetShoppingCartTotal()
        {
            var total = GetShoppingCartItems().Where(n => n.ShoppingCartId == ShoppingCartId).Select(n => n.ProductPrice * n.Amount).Sum();
            return (double)total;
        }
    }
}
