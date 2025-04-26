namespace EcommerceWebApp.Models.Dtos
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; }
        public string About { get; set; } = default!;
        public int SubcategoryId { get; set; }
    }
}
