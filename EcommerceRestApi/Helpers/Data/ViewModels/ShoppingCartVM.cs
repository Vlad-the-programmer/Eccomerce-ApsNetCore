using EcommerceRestApi.Models;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class ShoppingCartVM
    {
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }
        public double CartTotal { get; set; }
    }
}
