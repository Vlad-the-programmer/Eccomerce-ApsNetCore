namespace EcommerceRestApi.Models.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        public int? CustomerId { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime DateCreated { get; set; }
    }
}
