namespace EcommerceWebApp.Models.Dtos
{
    public class ShoppingCartDTO : BaseViewModel
    {
        public ShoppingCartDTO() : base("Cart") { }

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItemDTO> ShoppingCartItems { get; set; } = new List<ShoppingCartItemDTO>();
        public decimal CartTotal { get; set; } = decimal.Zero;

    }
}
