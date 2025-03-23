namespace EcommerceWebApp.Models
{
    public class CountryViewModel: BaseViewModel
    {
        public CountryViewModel() : base("Countries") { }
        public int Id { get; set; }
        public string CountryCode { get; set; } = default!;
        public string CountryName { get; set; } = default!;
    }
}
