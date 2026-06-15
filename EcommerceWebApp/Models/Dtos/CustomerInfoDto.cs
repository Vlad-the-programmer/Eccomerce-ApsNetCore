namespace EcommerceWebApp.Models.Dtos
{
    public class CustomerInfoDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Nip { get; set; }
    }
}
