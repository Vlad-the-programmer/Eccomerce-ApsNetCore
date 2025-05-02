using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.Dtos
{
    public class ShoppingCartItemDTO
    {

        public int ProductId { get; set; } // Foreign key property


        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Amount must be 0 or greater.")]
        public int Amount { get; set; }

        [Required]
        public string ShoppingCartId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

    }
}