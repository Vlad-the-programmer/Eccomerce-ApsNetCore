using EcommerceWebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceRestApi.Models
{
    public class ShoppingCartItemVM: BaseViewModel
    {

        public int ProductId { get; set; } // Foreign key property

        // Navigation property to the Product model
        [ForeignKey("ProductId")] // Explicitly specify the foreign key
        [InverseProperty("ShoppingCartItems")]
        public NewProductViewModel Product { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Amount must be 0 or greater.")]
        public int Amount { get; set; }

        [Required]
        public string ShoppingCartId { get; set; }

        [JsonIgnore]
        public double TotalPrice { get; set; }
    }
}