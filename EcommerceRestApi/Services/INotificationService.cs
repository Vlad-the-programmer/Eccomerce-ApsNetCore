using EcommerceRestApi.Models.Dtos;

namespace EcommerceRestApi.Services
{
    public interface INotificationService
    {
        Task AddNotificationForCustomerAsync(int customerId, string message);
        Task AddNotificationForUserAsync(string userId, string message);

        Task<List<NotificationDto>> GetNotificationsForCustomerAsync(int customerId);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task<List<NotificationDto>> GetNotificationsForUserAsync(string userId);

    }
}
