namespace EcommerceRestApi.Models.Dtos
{
    public class CreateReturnDto
    {
        public string Reason { get; set; }
        public decimal? RefundAmount { get; set; }
    }
}
