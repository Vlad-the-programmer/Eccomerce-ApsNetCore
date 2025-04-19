using EcommerceRestApi.Helpers.ModelsUtils;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;

namespace Inventory_Management_Sustem.Models.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = null!;
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public string? UserName { get; set; }

        public CustomerDto Customer { get; set; } = null!;
        public ProductDto Product { get; set; } = null!;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;

        //public static implicit operator Review(ReviewDto review)
        //   => new Review().CopyProperties(review);

        //public static implicit operator ReviewDto(Review review)
        //    => new Review().CopyProperties(review);

        public static ReviewDto FromEntity(Review review, bool includeProduct = true)
        {
            if (review == null) return new();
            var dto = new ReviewDto();
            dto.CopyProperties(review);

            dto.Customer = CustomerDto.ToDto(review.Customer);
            dto.UserName = review.Customer.User.UserName;

            if (includeProduct)
            {
                dto.Product = ProductDto.ToDto(review.Product, includeReviews: false);
            }
            return dto;
            //return new ReviewDto
            //{
            //    Id = review.Id,
            //    ReviewText = review.ReviewText,
            //    Rating = review.Rating,
            //    Customer = CustomerDto.ToDto(review.Customer),
            //    Product = ProductDto.ToDto(review.Product),
            //    CustomerId = review.CustomerId,
            //    ProductId = review.ProductId,
            //    DateCreated = review.DateCreated,
            //    DateUpdated = review.DateUpdated
            //};
        }
    }
}
