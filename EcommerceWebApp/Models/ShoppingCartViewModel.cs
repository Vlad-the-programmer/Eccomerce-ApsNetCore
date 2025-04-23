using EcommerceRestApi.Models;

namespace EcommerceWebApp.Models
{
    public class ShoppingCartViewModel : BaseViewModel
    {
        public ShoppingCartViewModel() : base("Cart") { }

        public List<ShoppingCartItemVM> ShoppingCartItems { get; set; } = new List<ShoppingCartItemVM>();
        public decimal CartTotal { get; set; } = decimal.Zero;

    }
}
