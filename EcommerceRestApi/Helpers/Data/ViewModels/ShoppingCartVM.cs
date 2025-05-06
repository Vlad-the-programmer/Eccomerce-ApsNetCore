using EcommerceRestApi.Models.Dtos;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class ShoppingCartVM
    {
        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItemDTO> ShoppingCartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
