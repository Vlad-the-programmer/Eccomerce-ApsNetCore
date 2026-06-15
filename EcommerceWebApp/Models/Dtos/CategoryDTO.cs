namespace EcommerceWebApp.Models.Dtos
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; } = default!;
        public string Name { get; set; }
        public string? About { get; set; } = default!;
        public int SubcategoryId { get; set; }
        public bool IsActive { get; set; }
        public List<SubcategoryDTO> Subcategories { get; set; } = new List<SubcategoryDTO>();
        public DateTime? DateDeleted { get; set; }
    }
}
