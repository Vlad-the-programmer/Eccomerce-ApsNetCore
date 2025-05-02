using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Models.Dtos
{
    public class ShoppingCartItemDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public string ShoppingCartId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}