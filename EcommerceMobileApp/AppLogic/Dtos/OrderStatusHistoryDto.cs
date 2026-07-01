namespace EcommerceMobileApp.AppLogic.Dtos
{
    public class OrderStatusHistoryDto
    {
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsCurrent { get; set; } = false;

    }
}
