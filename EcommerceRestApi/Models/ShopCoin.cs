namespace EcommerceRestApi.Models
{
    public class ShopCoin : EntityBase
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int Balance { get; set; } = 0;
    }
}
