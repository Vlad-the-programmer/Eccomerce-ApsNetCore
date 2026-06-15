using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.Dtos
{
    public class ProductAttributeDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Color", "Size", "Material"

        public string? Description { get; set; }

        [Required]
        public int DisplayOrder { get; set; }
        public ICollection<ProductAttributeValueDTO> ProductAttributeValues { get; set; } = new List<ProductAttributeValueDTO>();
    }
}
