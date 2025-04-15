using EcommerceRestApi.Helpers.ModelsUtils;
using EcommerceRestApi.Models;

namespace Inventory_Management_Sustem.Models.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;

        public static implicit operator Review(ReviewDto product)
           => new Review().CopyProperties(product);

        public static implicit operator ReviewDto(Review product)
            => new Review().CopyProperties(product);

        public static ReviewDto FromEntity(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                Customer = review.Customer,
                Product = review.Product,
                DateCreated = review.DateCreated,
                DateUpdated = review.DateUpdated
            };
        }
    }
}
