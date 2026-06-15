using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {

        private readonly ICouponService _service;
        public CouponController(AppDbContext dbContext, ICouponService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _service.GetAllCoupons();
            return Ok(coupons);
        }

        [HttpGet("admin")]
        [Authorize(Policy = Permissions.ManageCoupons)]
        public async Task<IActionResult> GetAllCouponsForAdmin()
        {
            var coupons = await _service.GetAllCouponsForAdmin();
            return Ok(coupons);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var coupon = await _service.GetCouponyByCodeAsync(code);
            if (coupon == null)
            {
                return NotFound();
            }
            return Ok(coupon);
        }

        [HttpPost]
        [Authorize(Policy = Permissions.ManageCoupons)]
        public async Task<IActionResult> Create([FromBody] CouponCreateUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _service.CreateCouponAsync(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel
                {
                    Message = ex.Message,
                });

            }
            return Created();
        }

        [HttpPut("{code}")]
        [Authorize(Policy = Permissions.ManageCoupons)]
        public async Task<IActionResult> Update(string code, [FromBody] CouponCreateUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                }).ToList();

                return BadRequest(new ResponseModel
                {
                    Message = "Validation failed",
                    Errors = (IList<string>)errors
                });
            }


            var coupon = await _service.GetCouponyByCodeAsync(code);
            if (coupon == null)
            {
                return NotFound();
            }

            try
            {
                await _service.UpdateCouponAsync(code, model);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel
                {
                    Message = ex.Message,
                });

            }
            return NoContent();
        }

        [HttpPost("remove-coupon/{code}")]
        public async Task<IActionResult> RemoveCoupon(string code)
        {
            var coupon = await _service.GetCouponyByCodeAsync(code);
            if (coupon == null)
            {
                return NotFound();
            }

            await _service.RemoveCouponAsync(code);
            return NoContent();
        }

        [HttpDelete("{code}")]
        [Authorize(Policy = Permissions.ManageCoupons)]
        public async Task<IActionResult> DeleteCoupon(string code)
        {
            var coupon = await _service.GetCouponyByCodeAsync(code);

            if (coupon == null)
            {
                return NotFound();
            }

            await _service.DeleteCouponAsync(code);
            return NoContent();
        }

        [HttpPost("apply/{couponCode}")]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var result = await _service.ApplyCouponAsync(couponCode);
            if (!result.Success)
            {
                return BadRequest(new CouponResult
                {
                    Success = false,
                    Message = result.Message
                });
            }
            return Ok(new CouponResult
            {
                Success = true,
                Discount = result.Discount,
                Message = "Coupon applied successfully"
            });
        }

        public class CouponResult
        {
            public bool Success { get; set; }
            public decimal Discount { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
