namespace EcommerceWebApp.Models.Dtos
{
    public class RefundStatusHistoryDto
    {
        public string RefundCode { get; set; }

        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
