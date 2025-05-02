namespace EcommerceWebApp.Models.Dtos
{
    public class ProductCategoryDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public virtual CategoryDTO Category { get; set; } = null!;

        public virtual NewProductViewModel Product { get; set; } = null!;
    }
}
