namespace EcommerceRestApi.Models
{
    public class WishlistItem : EntityBase
    {
        public int Id { get; set; }
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public string? Notes { get; set; }

        public Wishlist Wishlist { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
