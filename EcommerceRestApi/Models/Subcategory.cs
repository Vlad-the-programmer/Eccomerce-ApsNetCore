using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models
{
    public class Subcategory : EntityBase
    {

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Code must be between 3 and 20 characters.")]
        public string Code { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Subcategory name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string About { get; set; } = default!;

        public int CategoryId { get; set; }

        // Many-to-One: A Subcategory belongs to a Category
        [ForeignKey("CategoryId")]
        [InverseProperty("Subcategories")]
        public virtual Category Category { get; set; } = null!;

        // One-to-Many: A Subcategory has multiple Products
        [InverseProperty("Subcategory")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
