using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models
{
    public class Product : EntityBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Brand must be between 2 and 50 characters.")]
        public string Brand { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Code cannot exceed 30 characters.")]
        public string Code { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be 0 or greater.")]
        public int Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Old Price must be 0 or greater.")]
        public int? OldPrice { get; set; } = default!;

        [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters.")]
        public string? About { get; set; } = default!;

        [Required]
        [StringLength(2000, ErrorMessage = "Long description cannot exceed 2000 characters.")]
        public string LongAbout { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "RatingSum cannot be negative.")]
        public int? RatingSum { get; set; } = default!;

        [Range(0, int.MaxValue, ErrorMessage = "RatingVotes cannot be negative.")]
        public int? RatingVotes { get; set; } = default!;

        [StringLength(255, ErrorMessage = "Photo path cannot exceed 255 characters.")]
        public string? Photo { get; set; } = default!;

        [StringLength(1000, ErrorMessage = "Other photos paths cannot exceed 1000 characters.")]
        public string? OtherPhotos { get; set; } = default!;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be 0 or greater.")]
        public int Stock { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Subcategory ID.")]
        public int SubcategoryId { get; set; }

        // Many-to-One: A Product belongs to one Subcategory
        [ForeignKey("SubcategoryId")]
        [InverseProperty("Products")]
        public virtual Subcategory Subcategory { get; set; } = null!;

        // Many-to-Many: A Product can belong to multiple Categories (via ProductCategory)
        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        [InverseProperty("Product")]
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

        [InverseProperty("Product")]
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        
        [InverseProperty("Product")]
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
