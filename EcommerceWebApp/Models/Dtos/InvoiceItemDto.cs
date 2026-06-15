namespace EcommerceWebApp.Models.Dtos
{
    public class InvoiceItemDto
    {
        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        public decimal BasePricePerUnit { get; set; }

        public double TaxRate { get; set; }

        public double Quantity { get; set; }

        public double Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductBrand { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;


        //public virtual Invoice Invoice { get; set; } = null!;

        //public ProductDTO Product { get; set; } = null!;
    }
}
