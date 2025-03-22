namespace EcommerceWebApp.Models
{
    public class CountryViewModel: BaseViewModel
    {
        public CountryViewModel() : base("Countries") { }
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}
