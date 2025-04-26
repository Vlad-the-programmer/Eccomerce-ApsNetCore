namespace EcommerceWebApp.Models.Dtos
{
    public class CountryDTO
    {
        public int Id { get; set; }
        public string CountryCode { get; set; } = default!;
        public string CountryName { get; set; } = default!;
    }
}
