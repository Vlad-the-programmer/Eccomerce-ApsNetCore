using EcommerceRestApi.Helpers.ModelsUtils;
using EcommerceRestApi.Models;

namespace Inventory_Management_Sustem.Models.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Brand { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Photo { get; set; } = default!;
        public string? OtherPhotos { get; set; } = default!;
        public string? About { get; set; } = default!;
        public string LongAbout { get; set; }
        public int? RatingSum { get; set; } = default!;
        public int? RatingVotes { get; set; } = default!;
        public string SubcategoryCode { get; set; }
        public string CategoryCode { get; set; }
        public bool IsActive { get; set; }
        public IList<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();

        public static ProductDto ToDto(Product product)
        {
            var dto = new ProductDto().CopyProperties(product);
            dto.SubcategoryCode = product.Subcategory?.Code ?? string.Empty;
            dto.CategoryCode = product.ProductCategories.FirstOrDefault()?.Category?.Code ?? string.Empty;
            dto.Reviews = product.Reviews?.Select(ReviewDto.FromEntity).ToList() ?? new();
            dto.RatingSum = product.RatingSum ?? 0;
            dto.RatingVotes = product.RatingVotes ?? 0;
            return dto;
        }
    }
}
