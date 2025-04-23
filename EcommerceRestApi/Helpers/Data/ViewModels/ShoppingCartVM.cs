using EcommerceRestApi.Models;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class ShoppingCartVM
    {
        public List<ShoppingCartItemVM> ShoppingCartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
