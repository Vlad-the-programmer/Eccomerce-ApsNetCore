using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationForCustomerAsync(int customerId, string message)
        {
            var notification = new Notification
            {
                CustomerId = customerId,
                Message = message,
                IsRead = false,
                DateCreated = DateTime.UtcNow
            };

            await _context.Notification.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationForUserAsync(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                DateCreated = DateTime.UtcNow
            };

            await _context.Notification.AddAsync(notification);
            await _context.SaveChangesAsync();

        }

        public async Task<List<NotificationDto>> GetNotificationsForCustomerAsync(int customerId)
        {
            return await _context.Notification
                .Where(n => n.CustomerId == customerId && !n.IsRead)
                .OrderByDescending(n => n.DateCreated)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    CustomerId = n.CustomerId,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    DateCreated = n.DateCreated
                })
                .ToListAsync();
        }

        public async Task<List<NotificationDto>> GetNotificationsForUserAsync(string userId)
        {
            return await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.DateCreated)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    DateCreated = n.DateCreated
                })
                .ToListAsync();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.Notification
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
                throw new Exception("Notification not found");

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.IsActive = false;
                notification.DateDeleted = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}