using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace EcommerceWebApp.Helpers.ViewComponents
{
    public class NotificationSummary : ViewComponent
    {
        public readonly IApiService _apiService;

        public NotificationSummary(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new NotificationSummaryViewModel();

            try
            {
                var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                string response = "";

                if (role == "User")
                {
                    var customerId = HttpContext.User.FindFirst("CustomerId")?.Value;
                    response = await _apiService.GetDataAsync($"api/notification/customer/{customerId}");
                }
                else if (role == "Admin" || role == "Stuff")
                {
                    response = await _apiService.GetDataAsync("api/notification/current-user");
                }

                var items = JsonSerializer.Deserialize<List<NotificationDto>>(response, GlobalConstants.JsonSerializerOptions) ?? new();

                model.Count = items.Count;
                model.Items = items.Take(5).ToList();
            }
            catch
            {
                model.Count = 0;
            }

            return View(model);
        }
    }
}
