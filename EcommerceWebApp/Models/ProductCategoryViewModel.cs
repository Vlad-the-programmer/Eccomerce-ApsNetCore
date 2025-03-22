using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public virtual CategoryViewModel Category { get; set; } = null!;

        public virtual NewProductViewModel Product { get; set; } = null!;
    }
}
