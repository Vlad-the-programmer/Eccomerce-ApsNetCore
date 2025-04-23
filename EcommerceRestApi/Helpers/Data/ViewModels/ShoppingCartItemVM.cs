using EcommerceRestApi.Helpers.Data.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Models
{
    public class ShoppingCartItemVM : BaseViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; } // Foreign key property

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