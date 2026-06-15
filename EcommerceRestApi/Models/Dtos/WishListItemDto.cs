namespace EcommerceRestApi.Models.Dtos
{
    public class WishListItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WishlistId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductBrand { get; set; }
        public decimal Price { get; set; }

    }
}
