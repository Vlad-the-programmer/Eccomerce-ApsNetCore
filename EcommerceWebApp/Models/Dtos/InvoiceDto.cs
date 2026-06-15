namespace EcommerceWebApp.Models.Dtos
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime DateOfIssue { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountTotal { get; set; }

        public CustomerInfoDto Customer { get; set; }
        public List<InvoiceItemDto> InvoiceItems { get; set; } = new List<InvoiceItemDto>();
    }
}
