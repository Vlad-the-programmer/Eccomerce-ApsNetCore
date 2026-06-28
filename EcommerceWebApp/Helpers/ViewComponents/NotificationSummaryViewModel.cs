using EcommerceWebApp.Models.Dtos;

namespace EcommerceWebApp.Helpers.ViewComponents
{
    public class NotificationSummaryViewModel
    {
        public int Count { get; set; }
        public List<NotificationDto> Items { get; set; } = new();
    }
}
