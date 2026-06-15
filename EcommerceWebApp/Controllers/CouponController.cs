using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("coupons")]
    public class CouponController : Controller
    {
        private readonly IApiService _apiService;

        public CouponController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Coupon/Index
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetDataAsync(GlobalConstants.CouponEndpoint);
                var coupons = JsonSerializer.Deserialize<List<CouponDto>>(response, GlobalConstants.JsonSerializerOptions);
                return View(coupons);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API Error: {ex.Message}";
                return View(new List<CouponDto>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return View(new List<CouponDto>());
            }
        }

        // GET: Coupon/All-coupons-admin
        [HttpGet("admin-coupons")]
        public async Task<IActionResult> All()
        {
            try
            {
                var response = await _apiService.GetDataAsync($"{GlobalConstants.CouponEndpoint}/admin");
                var coupons = JsonSerializer.Deserialize<List<CouponDto>>(response, GlobalConstants.JsonSerializerOptions);
                return View(coupons);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API Error: {ex.Message}";
                return View(new List<CouponDto>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return View(new List<CouponDto>());
            }
        }

        // GET: Coupon/Details/{code}
        [HttpGet("coupons/details/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string code)
        {
            try
            {
                var response = await _apiService.GetDataAsync($"{GlobalConstants.CouponEndpoint}/{code}");
                var coupon = JsonSerializer.Deserialize<CouponDto>(response, GlobalConstants.JsonSerializerOptions);

                if (coupon == null)
                {
                    TempData["Error"] = "Coupon not found";
                    return RedirectToAction(nameof(All));
                }

                return View(coupon);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API Error: {ex.Message}";
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return RedirectToAction(nameof(All));
            }
        }

        // GET: Coupon/Create
        [HttpGet("coupons/create")]
        public IActionResult Create()
        {
            return View(new CouponCreateUpdateDTO
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                UsageLimit = 1,
                DiscountAmount = 0
            });
        }

        // POST: Coupon/Create
        [HttpPost("coupons/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponCreateUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiService.PostDataAsync(GlobalConstants.CouponEndpoint,
                    JsonSerializer.Serialize(model, GlobalConstants.JsonSerializerOptions));

                TempData["Success"] = "Coupon created successfully!";
                return RedirectToAction(nameof(All));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API Error: {ex.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return View(model);
            }
        }

        // GET: Coupon/Edit/{code}
        [HttpGet("coupons/edit/{code}")]
        public async Task<IActionResult> Edit(string code)
        {
            try
            {
                var response = await _apiService.GetDataAsync($"{GlobalConstants.CouponEndpoint}/{code}");
                var coupon = JsonSerializer.Deserialize<CouponDto>(response, GlobalConstants.JsonSerializerOptions);

                if (coupon == null)
                {
                    TempData["Error"] = "Coupon not found";
                    return RedirectToAction(nameof(All));
                }

                var editModel = new CouponCreateUpdateDTO
                {
                    Code = coupon.Code,
                    Description = coupon.Description,
                    DiscountAmount = coupon.DiscountAmount,
                    DiscountPercentage = coupon.DiscountPercentage,
                    MinOrderAmount = coupon.MinOrderAmount,
                    MaxDiscountAmount = coupon.MaxDiscountAmount,
                    StartDate = coupon.StartDate,
                    EndDate = coupon.EndDate,
                    UsageLimit = coupon.UsageLimit,
                    PerUserLimit = coupon.PerUserLimit
                };

                return View(editModel);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(All));
            }
        }

        // POST: Coupon/Edit/{code}
        [HttpPost("coupons/edit/{code}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string code, CouponCreateUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiService.UpdateDataAsync($"{GlobalConstants.CouponEndpoint}/{code}",
                    JsonSerializer.Serialize(model, GlobalConstants.JsonSerializerOptions));

                TempData["Success"] = "Coupon updated successfully!";
                return RedirectToAction(nameof(All));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API Error: {ex.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return View(model);
            }
        }

        // POST: Coupon/Delete/{code}
        [HttpPost("coupons/delete/{code}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.CouponEndpoint}/{code}");
                TempData["Success"] = "Coupon deleted successfully!";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"API Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
            }

            return RedirectToAction(nameof(All));
        }

        // POST: Coupon/Apply (for customers)
        [HttpPost("apply-coupon")]
        [AllowAnonymous]
        public async Task<IActionResult> ApplyCoupon([FromForm] string couponCode)
        {
            try
            {
                var resultJson = await _apiService.PostDataAsync($"{GlobalConstants.ApplyCouponEndpoint}/{couponCode}");
                var result = JsonSerializer.Deserialize<CouponResult>(resultJson, GlobalConstants.JsonSerializerOptions);

                if (result != null && result.Success)
                {
                    TempData["CouponDiscount"] = result.Discount.ToString(CultureInfo.InvariantCulture);
                    TempData["AppliedCouponCode"] = couponCode;
                    TempData["Success"] = result.Message ?? "Coupon applied successfully!";

                    return Ok(new { success = true, message = result.Message ?? "Coupon applied successfully!", discount = result.Discount });
                }
                else
                {
                    return BadRequest(new { success = false, message = result?.Message ?? "Failed to apply coupon" });
                }
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (JsonException ex)
            {
                return BadRequest(new { success = false, message = $"Invalid response from server: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Unexpected error: {ex.Message}" });
            }
        }

        // POST: Coupon/Remove
        [HttpPost("remove-coupon")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveCoupon(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                code = TempData["AppliedCouponCode"]?.ToString();
            }

            if (string.IsNullOrEmpty(code))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, message = "No coupon code found to remove" });
                }
                TempData["Error"] = "No coupon code found to remove";
                return RedirectToAction("Index", "Orders");
            }

            try
            {
                var url = $"{GlobalConstants.RemoveCouponEndpoint}/{code}";
                await _apiService.PostDataAsync(url);

                TempData["CouponDiscount"] = 0;
                TempData["AppliedCouponCode"] = null;

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Ok(new { success = true, message = "Coupon removed successfully" });
                }

                TempData["Success"] = "Coupon removed successfully";
                return RedirectToAction("Index", "Orders");
            }
            catch (HttpRequestException ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, message = $"API Error: {ex.Message}" });
                }
                TempData["Error"] = $"API Error: {ex.Message}";
                return RedirectToAction("Index", "Orders");
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, message = $"Unexpected error: {ex.Message}" });
                }
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return RedirectToAction("Index", "Orders");
            }
        }
    }

    public class CouponResult
    {
        public bool Success { get; set; }
        public decimal Discount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}