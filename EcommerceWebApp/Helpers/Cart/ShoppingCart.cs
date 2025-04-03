using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models;
using EcommerceRestApi.Helpers.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApp.Models;

namespace EcommerceWebApp.Helpers.Cart
{
    public class ShoppingCart
    {

        public string ShoppingCartId { get; set; }

        public List<ShoppingCartItemVM> ShoppingCartItems { get; set; }

        public ShoppingCart()
        {
        }

        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            return new ShoppingCart();
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
            var total = GetShoppingCartItems().Where(n => n.ShoppingCartId == ShoppingCartId).Select(n => n.Product.Price * n.Amount).Sum();
            return total;
        }
    }
}
