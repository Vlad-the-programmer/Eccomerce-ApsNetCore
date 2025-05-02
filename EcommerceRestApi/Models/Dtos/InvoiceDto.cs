namespace EcommerceRestApi.Models.Dtos
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime DateOfIssue { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public string CustomerName { get; set; } = string.Empty; // Derived from Customer
    }
}
