namespace EcommerceMobileApp.AppLogic.Dtos
{
    public class ShopCoinTransactionHistoryDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        //public CustomerDto Customer { get; set; }

        public int Coins { get; set; }

        public string Type { get; set; } = null!; // "EARN_ORDER", "SPEND_ORDER", "BONUS", "REFUND"

        public string? Description { get; set; }

        public int? OrderId { get; set; }
        //public OrderDto? Order { get; set; }
        public string OrderCode { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
