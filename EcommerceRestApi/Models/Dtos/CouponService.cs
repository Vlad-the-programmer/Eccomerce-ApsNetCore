using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;
using EcommerceRestApi.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models.Dtos
{
    public class CouponService : EntityBaseRepository<Coupon>, ICouponService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ShoppingCart _cart;

        public CouponService(AppDbContext context,
            IHttpContextAccessor httpContextAccessor, ShoppingCart cart) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _cart = cart;
        }

        private ISession Session => _httpContextAccessor.HttpContext?.Session;

        public async Task<(bool Success, decimal Discount, string Message)> ApplyCouponAsync(string couponCode)
        {
            var cart = _cart.GetShoppingCart();
            var cartTotal = await cart.GetTotal();

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == couponCode && c.IsActive);

            if (coupon == null)
            {
                return (false, 0, "Invalid coupon code");
            }

            if (coupon.StartDate > DateTime.Now)
            {
                return (false, 0, "Coupon is not yet active");
            }

            if (coupon.EndDate < DateTime.Now)
            {
                return (false, 0, "Coupon has expired");
            }

            if (coupon.UsageLimit <= coupon.UsedCount)
            {
                return (false, 0, "Coupon usage limit has been reached");
            }

            if (coupon.MinOrderAmount.HasValue && cartTotal < coupon.MinOrderAmount.Value)
            {
                return (false, 0, $"Minimum order amount of ${coupon.MinOrderAmount.Value:F2} required");
            }

            decimal discount = 0;
            if (coupon.DiscountPercentage.HasValue)
            {
                discount = cartTotal * (coupon.DiscountPercentage.Value / 100);
                if (coupon.MaxDiscountAmount.HasValue && discount > coupon.MaxDiscountAmount.Value)
                {
                    discount = coupon.MaxDiscountAmount.Value;
                }
            }
            else
            {
                discount = coupon.DiscountAmount;
            }

            // Store coupon in session
            if (Session != null)
            {
                Session.SetString("AppliedCouponCode", couponCode);
                Session.SetString("CouponDiscount", discount.ToString());
            }

            return (true, discount, "Coupon applied successfully");
        }

        public async Task<List<CouponDto>> GetAllCoupons()
        {
            var coupons = await _context.Coupons
                .Where(c => c.IsActive)
                .Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = c.DiscountAmount,
                    DiscountPercentage = c.DiscountPercentage,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscountAmount = c.MaxDiscountAmount,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    UsageLimit = c.UsageLimit,
                    UsedCount = c.UsedCount,
                    PerUserLimit = c.PerUserLimit,
                    IsActive = c.IsActive,
                    DateCreated = c.DateCreated,
                    DateUpdated = c.DateUpdated,
                    DateDeleted = c.DateDeleted
                })
                .ToListAsync();

            return coupons;
        }

        public async Task<List<CouponDto>> GetAllCouponsForAdmin()
        {
            // For admin, show all coupons including inactive/deleted ones
            var coupons = await _context.Coupons
                .Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = c.DiscountAmount,
                    DiscountPercentage = c.DiscountPercentage,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscountAmount = c.MaxDiscountAmount,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    UsageLimit = c.UsageLimit,
                    UsedCount = c.UsedCount,
                    PerUserLimit = c.PerUserLimit,
                    IsActive = c.IsActive,
                    DateCreated = c.DateCreated,
                    DateUpdated = c.DateUpdated,
                    DateDeleted = c.DateDeleted
                })
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync();

            return coupons;
        }

        public async Task CreateCouponAsync(CouponCreateUpdateDTO data)
        {
            // Check if coupon with same code already exists
            var existingCoupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == data.Code && c.IsActive);

            if (existingCoupon != null)
            {
                throw new Exception("Coupon with such code already exists!");
            }

            if (data.StartDate >= data.EndDate)
            {
                throw new Exception("Start date must be before end date");
            }

            if (data.DiscountAmount <= 0 && (!data.DiscountPercentage.HasValue || data.DiscountPercentage.Value <= 0))
            {
                throw new Exception("Either discount amount or discount percentage must be specified");
            }

            if (data.DiscountPercentage.HasValue && (data.DiscountPercentage.Value < 0 || data.DiscountPercentage.Value > 100))
            {
                throw new Exception("Discount percentage must be between 0 and 100");
            }

            var coupon = new Coupon
            {
                Code = data.Code,
                Description = data.Description,
                DiscountAmount = data.DiscountAmount,
                DiscountPercentage = data.DiscountPercentage,
                MinOrderAmount = data.MinOrderAmount,
                MaxDiscountAmount = data.MaxDiscountAmount,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                UsageLimit = data.UsageLimit > 0 ? data.UsageLimit : 1,
                UsedCount = 0,
                PerUserLimit = data.PerUserLimit,
                IsActive = true,
                DateCreated = DateTime.Now
            };

            await _context.Coupons.AddAsync(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCouponAsync(string code, CouponCreateUpdateDTO data)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);

            if (coupon == null)
            {
                throw new Exception("Coupon not found");
            }

            var existingCoupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == data.Code && c.Code != code && c.IsActive);

            if (existingCoupon != null)
            {
                throw new Exception("Another coupon with this code already exists!");
            }

            if (data.StartDate >= data.EndDate)
            {
                throw new Exception("Start date must be before end date");
            }

            coupon.Code = data.Code;
            coupon.Description = data.Description;
            coupon.DiscountAmount = data.DiscountAmount;
            coupon.DiscountPercentage = data.DiscountPercentage;
            coupon.MinOrderAmount = data.MinOrderAmount;
            coupon.MaxDiscountAmount = data.MaxDiscountAmount;
            coupon.StartDate = data.StartDate;
            coupon.EndDate = data.EndDate;
            coupon.UsageLimit = data.UsageLimit > 0 ? data.UsageLimit : 1;
            coupon.PerUserLimit = data.PerUserLimit;
            coupon.DateUpdated = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<CouponDto> GetCouponyByCodeAsync(string code)
        {
            var coupon = await _context.Coupons
                .Where(c => c.Code == code)
                .Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = c.DiscountAmount,
                    DiscountPercentage = c.DiscountPercentage,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscountAmount = c.MaxDiscountAmount,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    UsageLimit = c.UsageLimit,
                    UsedCount = c.UsedCount,
                    PerUserLimit = c.PerUserLimit,
                    IsActive = c.IsActive,
                    DateCreated = c.DateCreated,
                    DateUpdated = c.DateUpdated,
                    DateDeleted = c.DateDeleted
                })
                .FirstOrDefaultAsync();

            return coupon;
        }

        public async Task<CouponDto> GetCouponyByIdAsync(int id)
        {
            var coupon = await _context.Coupons
                .Where(c => c.Id == id)
                .Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = c.DiscountAmount,
                    DiscountPercentage = c.DiscountPercentage,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscountAmount = c.MaxDiscountAmount,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    UsageLimit = c.UsageLimit,
                    UsedCount = c.UsedCount,
                    PerUserLimit = c.PerUserLimit,
                    IsActive = c.IsActive,
                    DateCreated = c.DateCreated,
                    DateUpdated = c.DateUpdated,
                    DateDeleted = c.DateDeleted
                })
                .FirstOrDefaultAsync();

            return coupon;
        }

        public async Task RemoveCouponAsync(string code)
        {
            if (Session != null)
            {
                Session.Remove("AppliedCouponCode");
                Session.Remove("CouponDiscount");
            }
        }

        public async Task<decimal?> GetAppliedCouponDiscountAsync()
        {
            var discountStr = Session?.GetString("CouponDiscount");
            if (string.IsNullOrEmpty(discountStr))
                return null;

            return decimal.TryParse(discountStr, out var discount) ? discount : null;
        }

        public async Task<string> GetAppliedCouponCodeAsync()
        {
            return Session?.GetString("AppliedCouponCode");
        }

        public async Task DeleteCouponAsync(string code)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
            if (coupon != null)
            {
                coupon.IsActive = false;
                coupon.DateDeleted = DateTime.Now;
                coupon.DateUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}