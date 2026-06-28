namespace EcommerceWebApp.Models.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? UserId { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
