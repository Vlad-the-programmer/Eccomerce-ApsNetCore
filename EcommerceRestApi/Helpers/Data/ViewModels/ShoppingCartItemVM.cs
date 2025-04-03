using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceWebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    public class ShoppingCartItemVM: BaseViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; } // Foreign key property

        public NewProductViewModel Product { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be 0 or greater.")]
        public int Amount { get; set; }

        [StringLength(20, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 20 characters.")]
        [Required]
        public string ShoppingCartId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}