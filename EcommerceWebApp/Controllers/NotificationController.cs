using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceWebApp.Controllers
{
    [Route("notifications")]
    public class NotificationController : Controller
    {
        private readonly IApiService _apiService;

        public NotificationController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetDataAsync($"api/notification/customer/{User.FindFirstValue("CustomerId")}");
                var notifications = System.Text.Json.JsonSerializer.Deserialize<List<NotificationDto>>(response,
                    GlobalConstants.JsonSerializerOptions);

                return View(notifications);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "Something went wrong!";
                return View(new List<NotificationDto>());
            }
        }

        [HttpPost("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                await _apiService.PostDataAsync($"api/notification/mark-as-read/{notificationId}");
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "Something went wrong!";
                return RedirectToAction("Index");
            }
        }

        //[HttpPost("add")]
        //public async Task<IActionResult> AddNotification(int customerId, [FromBody] NotificationDto dto)
        //{
        //    try
        //    {
        //        await _apiService.PostDataAsync($"api/notifications/add/{customerId}", dto);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        TempData["Error"] = "Something went wrong!";
        //    }
        //    return RedirectToAction("Index");
        //}
    }
}
