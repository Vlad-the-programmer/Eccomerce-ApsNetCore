using EcommerceRestApi.Models;

namespace EcommerceWebApp.Models
{
    public class ShoppingCartViewModel: BaseViewModel
    {
        public ShoppingCartViewModel() : base("Cart") { }

        public List<ShoppingCartItemVM> ShoppingCartItems { get; set; }
        public double CartTotal { get; set; }

    }
}
