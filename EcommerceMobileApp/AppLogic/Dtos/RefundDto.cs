namespace EcommerceMobileApp.AppLogic.Dtos
{
    public class RefundDto
    {
        public decimal Amount { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual IList<RefundItemDto> RefundItems { get; set; } = new List<RefundItemDto>();
        public virtual IList<RefundStatusHistoryDto> RefundStatusHistory { get; set; } = new List<RefundStatusHistoryDto>();

    }
}
