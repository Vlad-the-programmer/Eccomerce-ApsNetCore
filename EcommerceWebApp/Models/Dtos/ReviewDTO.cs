namespace EcommerceWebApp.Models.Dtos
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = null!;
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public string? UserName { get; set; }

        public CustomerDTO Customer { get; set; } = null!;
        public ProductDTO Product { get; set; } = null!;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }
}
