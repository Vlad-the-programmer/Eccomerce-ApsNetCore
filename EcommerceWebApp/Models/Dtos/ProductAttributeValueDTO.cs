using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.Dtos
{
    public class ProductAttributeValueDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int AttributeId { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty; // e.g., "Red", "Large", "Cotton"
        public decimal? PriceAdjustment { get; set; }
        public int? StockAdjustment { get; set; }
        public string? Sku { get; set; }

        public ProductDTO Product { get; set; } = null!;
        public ProductAttributeDTO Attribute { get; set; } = null!;
    }
}
