namespace EcommerceWebApp.Models.Dtos
{
    public class ReturnDto
    {
        public string Reason { get; set; }
        public string RefundCode { get; set; }
        public string Status { get; set; }

        public decimal? RefundAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
