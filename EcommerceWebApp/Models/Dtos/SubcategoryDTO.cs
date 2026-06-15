namespace EcommerceWebApp.Models.Dtos
{
    public class SubcategoryDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string About { get; set; } = default!;
        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? DateDeleted { get; set; }
        public IList<NewProductViewModel> Products { get; set; } = new List<NewProductViewModel>();
    }
}
