namespace EcommerceRestApi.Models
{
    public class Wishlist : EntityBase
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = "Default Wishlist";
        public bool IsDefault { get; set; }

        public Customer Customer { get; set; } = null!;
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
