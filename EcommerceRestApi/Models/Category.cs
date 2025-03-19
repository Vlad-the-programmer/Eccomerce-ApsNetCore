using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models
{
    public class Category : EntityBase
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 20 characters.")]
        [Required]
        public string Name { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 20 characters.")]
        public string About { get; set; } = default!;

        // One-to-Many: A Category has multiple Subcategories
        [InverseProperty("Category")]
        public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();

        // Many-to-Many: A Category can have multiple Products (through ProductCategory)
        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }

}
