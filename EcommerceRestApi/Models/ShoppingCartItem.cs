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
        [Range(0, int.MaxValue, ErrorMessage = "Amount must be 0 or greater.")]
        public int Amount { get; set; }

        [Required]
        public string ShoppingCartId { get; set; }

    }
}