namespace EcommerceWebApp.Models.Dtos
{
    public class OrderStatusHistoryDto
    {
        public string Status { get; set; }
        public string ChangedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
