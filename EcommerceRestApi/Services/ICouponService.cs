using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface ICouponService : IEntityBaseRepository<Coupon>
    {
        Task RemoveCouponAsync(string code);
        Task<(bool Success, decimal Discount, string Message)> ApplyCouponAsync(string couponCode);
        Task<List<CouponDto>> GetAllCoupons();
        Task<CouponDto> GetCouponyByCodeAsync(string code);
        Task<CouponDto> GetCouponyByIdAsync(int id);
        Task UpdateCouponAsync(string code, CouponCreateUpdateDTO data);
        Task CreateCouponAsync(CouponCreateUpdateDTO data);
        Task<List<CouponDto>> GetAllCouponsForAdmin();
        Task DeleteCouponAsync(string code);

    }
}
