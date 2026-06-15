
namespace EcommerceRestApi.Models.Dtos
{
    public class InvoiceItemDto
    {
        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        public decimal BasePricePerUnit { get; set; }

        public double TaxRate { get; set; }

        public double Quantity { get; set; }

        public double Discount { get; set; }

        //public virtual Invoice Invoice { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public string ProductBrand { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;


        //public Product Product { get; set; } = null!;
    }
}
