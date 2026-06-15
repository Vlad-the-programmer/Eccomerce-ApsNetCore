namespace EcommerceWebApp.Models
{
    public class CategoryTreeViewModel
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public List<SubcategoryTreeViewModel> Subcategories { get; set; } = new List<SubcategoryTreeViewModel>();
    }
}
