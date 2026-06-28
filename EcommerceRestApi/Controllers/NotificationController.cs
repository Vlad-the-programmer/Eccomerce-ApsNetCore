using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetNotificationsForCustomer(int customerId)
        {
            var notifications = await _notificationService.GetNotificationsForCustomerAsync(customerId);
            return Ok(notifications);
        }

        [HttpGet("current-user")]
        [Authorize($"{UserRoles.Admin}, {UserRoles.Stuff}")]
        public async Task<IActionResult> GetNotificationsForUser()
        {
            var notifications = await _notificationService.GetNotificationsForUserAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(notifications);
        }

        [HttpPost("mark-as-read/{notificationId}")]
        [Authorize]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }
    }
}
