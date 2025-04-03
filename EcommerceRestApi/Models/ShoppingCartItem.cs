using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    public class ShoppingCartItem : EntityBase
    {

        // Foreign key to the Product model
        [Required]
        public int ProductId { get; set; } // Foreign key property

        // Navigation property to the Product model
        [ForeignKey("ProductId")] // Explicitly specify the foreign key
        [InverseProperty("ShoppingCartItems")]
        public Product Product { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be 0 or greater.")]
        public int Amount { get; set; }

        [StringLength(20, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 20 characters.")]
        [Required]
        public string ShoppingCartId { get; set; }

    }
}