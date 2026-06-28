namespace EcommerceWebApp.Models.Dtos
{
    public class WishListDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = "Default Wishlist";
        public bool IsDefault { get; set; } = true;

        public ICollection<WishListItemDto> WishlistItems { get; set; } = new List<WishListItemDto>();
    }
}
