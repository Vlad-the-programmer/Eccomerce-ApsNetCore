using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class ShopCoinSettingsService : EntityBaseRepository<ShopCoinSettings>, IShopCoinSettingsService
    {
        private readonly AppDbContext _context;

        public ShopCoinSettingsService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateShopCoinSettingsAsync(ShopCoinSettingsDto dto)
        {
            var existing = await _context.ShopCoinSettings.AnyAsync();

            if (existing)
                throw new Exception("Settings already exist. Use update instead.");

            var entity = new ShopCoinSettings
            {
                EarnRate = dto.EarnRate,
                SpendRate = dto.SpendRate,
                MaxSpendPercentage = dto.MaxSpendPercentage,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _context.ShopCoinSettings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ShopCoinSettingsDto> GetShopCoinSettingsAsync()
        {
            var settings = await _context.ShopCoinSettings.FirstOrDefaultAsync();

            if (settings == null)
                throw new Exception("Shop coin settings not configured");

            return new ShopCoinSettingsDto
            {
                EarnRate = settings.EarnRate,
                SpendRate = settings.SpendRate,
                MaxSpendPercentage = settings.MaxSpendPercentage,
                LastUpdated = settings.DateUpdated
            };
        }

        public async Task UpdateShopCoinSettingsAsync(ShopCoinSettingsDto dto)
        {
            var settings = await _context.ShopCoinSettings.FirstOrDefaultAsync();

            if (settings == null)
                throw new Exception("Settings not found. Create them first.");

            settings.EarnRate = dto.EarnRate;
            settings.SpendRate = dto.SpendRate;
            settings.MaxSpendPercentage = dto.MaxSpendPercentage;
            settings.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
