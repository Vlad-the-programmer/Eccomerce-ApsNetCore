using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Helpers.Enums;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.AppViewModels;
using EcommerceWebApp.Models.UpdateViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("usersmanagement")]
    public class UsersManagementController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UsersManagementController> _logger;

        private const string BASE = "api/usersmanagement";

        public UsersManagementController(IApiService apiService,
                                         ILogger<UsersManagementController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetDataAsync($"{BASE}/get-staff-users");
                var users = JsonSerializer.Deserialize<List<CurrentUserDTO>>(response,
                    GlobalConstants.JsonSerializerOptions);

                ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                $"{BASE}/search-combo-box-dtos", _apiService);
                ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                    $"{BASE}/order-by-combo-box-dtos", _apiService);

                return View(users);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<CurrentUserDTO>());
            }
        }

        [HttpGet("permissions/{userId}")]
        public async Task<IActionResult> GetPermissions(string userId)
        {
            var response = await _apiService.GetDataAsync($"{BASE}/{userId}/permissions");
            var permissions = JsonSerializer.Deserialize<List<string>>(response,
                GlobalConstants.JsonSerializerOptions);

            return Json(permissions);
        }

        [HttpPost("bulk-update-permissions")]
        public async Task<IActionResult> BulkUpdatePermissions([FromBody] UpdatePermissionsDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);

                await _apiService.PostDataAsync($"{BASE}/bulk-update-permissions", json);

                return Ok();
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "Failed adding permissions";
                return BadRequest();
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var updateModel = await AccountEndpointsHelperFuncs.GetStaffUpdateModelObj(
                $"{BASE}/get-update-user-model/{id}", _apiService);

            if (updateModel == null)
            {
                return View("NotFound");
            }

            return View(updateModel);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, StaffUpdateVM updatedUser)
        {

            try
            {
                await _apiService.UpdateDataAsync($"{BASE}/{id}",
                                                    JsonSerializer.Serialize(updatedUser));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService);

                return View(updatedUser);
            }
            return RedirectToAction("Details", "Account");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _apiService.DeleteDataAsync($"{BASE}/{id}");
                TempData["Success"] = "Account deleted successfully";
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet("create-staff")]
        public async Task<IActionResult> CreateStaff()
        {
            return View(new CreateStaffVM());
        }

        [HttpPost("create-staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(CreateStaffVM registerVM)
        {
            try
            {
                var response = await _apiService.PostDataAsync(
                                    BASE,
                                    JsonSerializer.Serialize<CreateStaffVM>(registerVM));
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                return View(registerVM);
            }

            return View("Index");
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
        [FromQuery] string searchString = "",
        [FromQuery] string? searchProperty = null,
        [FromQuery] string? sortProperty = null,
        [FromQuery] string? statusFilter = null,
        [FromQuery] bool sortAscending = false)
        {
            try
            {
                var filterStatus = UserStatusFilter.All;
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    switch (statusFilter.ToLower())
                    {
                        case "active":
                            filterStatus = UserStatusFilter.ActiveOnly;
                            break;
                        case "inactive":
                            filterStatus = UserStatusFilter.InactiveOnly;
                            break;
                        case "all":
                        default:
                            filterStatus = UserStatusFilter.All;
                            break;
                    }
                }

                List<CurrentUserDTO> filteredStaff = await AccountEndpointsHelperFuncs.GetFilteredStaff(
                $"{BASE}/filter-staff", searchString, searchProperty, sortProperty, filterStatus, sortAscending, _apiService);

                ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                $"{BASE}/search-combo-box-dtos", _apiService);
                ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                    $"{BASE}/order-by-combo-box-dtos", _apiService);

                return View("Index", filteredStaff);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            ViewBag.CurrentStatusFilter = statusFilter;

            ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                $"{BASE}/search-combo-box-dtos", _apiService);
            ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                $"{BASE}/order-by-combo-box-dtos", _apiService);

            return RedirectToAction("Index", "UsersManagement");
        }

        public class UpdatePermissionsDto
        {
            public string UserId { get; set; }
            public List<string> Permissions { get; set; } = new();
        }
    }
}