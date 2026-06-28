using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class ShopCoinsService : IShopCoinsService
    {
        private readonly AppDbContext _context;
        private readonly IShopCoinSettingsService _settingsService;
        private readonly INotificationService _notificationService;

        public ShopCoinsService(AppDbContext context,
                IShopCoinSettingsService settingsService, INotificationService notificationService)
        {
            _context = context;
            _settingsService = settingsService;
            _notificationService = notificationService;
        }

        private async Task<int> GetCustomerIdAsync(string userId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                throw new Exception("Customer not found");

            return customer.Id;
        }

        public async Task<int> ConvertMoneyToCoins(decimal amount)
        {
            var settings = await _settingsService.GetShopCoinSettingsAsync();
            return (int)Math.Floor(amount * settings.EarnRate);
        }

        public async Task<decimal> ConvertFromCoinsToMoney(int coins)
        {
            var settings = await _settingsService.GetShopCoinSettingsAsync();
            return coins * settings.SpendRate;
        }

        private async Task<ShopCoin> GetOrCreateWallet(int customerId)
        {
            var wallet = await _context.ShopCoins.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (wallet == null)
            {
                wallet = new ShopCoin
                {
                    CustomerId = customerId,
                    Balance = 0
                };
                await _context.ShopCoins.AddAsync(wallet);
                await _context.SaveChangesAsync();
            }
            return wallet;
        }

        public async Task RewardCoinsForOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            var alreadyRewarded = await _context.ShopCoinTransactions
                .AnyAsync(t =>
                    t.OrderId == orderId &&
                    t.Type == ShopCoinTransactionType.EarnOrder.ToString());

            if (alreadyRewarded)
                return;

            int coinsEarned = await ConvertMoneyToCoins(order.TotalAmount);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var wallet = await _context.ShopCoins
                    .FirstOrDefaultAsync(w => w.CustomerId == order.CustomerId);

                if (wallet == null)
                    wallet = await GetOrCreateWallet(order.CustomerId);

                wallet.Balance += coinsEarned;

                await _context.ShopCoinTransactions.AddAsync(new ShopCoinTransactionHistory
                {
                    CustomerId = order.CustomerId,
                    Coins = coinsEarned,
                    Type = ShopCoinTransactionType.EarnOrder.ToString(),
                    OrderId = order.Id,
                    Description = $"Coins earned from order #{order.Code}"
                });

                await _context.SaveChangesAsync();

                await ChangeCustomerPoints(order.CustomerId);

                await transaction.CommitAsync();

                var message = $"You got {coinsEarned} coins for the order #{order.Code}!";
                await _notificationService.AddNotificationForCustomerAsync(order.CustomerId, message);

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ChangeCustomerPoints(int customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer != null)
            {
                var wallet = await _context.ShopCoins.FirstOrDefaultAsync(w => w.CustomerId == customerId);
                if (wallet != null)
                {
                    customer.Points = wallet.Balance;
                    await _context.SaveChangesAsync();

                    var message = $"Your wallet balance is now {wallet.Balance} coins!";
                    await _notificationService.AddNotificationForCustomerAsync(wallet.CustomerId, message);
                }
            }
        }

        public async Task<int> CalculateMaxCoinsToSpend(int customerId, decimal totalAmount)
        {
            var wallet = await GetOrCreateWallet(customerId);
            var settings = await _settingsService.GetShopCoinSettingsAsync();

            decimal maxDiscountMoney = totalAmount * settings.MaxSpendPercentage;

            int maxCoins = (int)Math.Floor(
                maxDiscountMoney * settings.SpendRate
            );

            return Math.Min(maxCoins, wallet.Balance);
        }

        public async Task SpendCoinsForOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            var wallet = await _context.ShopCoins
                .FirstOrDefaultAsync(w => w.CustomerId == order.CustomerId);

            if (wallet == null)
                wallet = await GetOrCreateWallet(order.CustomerId);

            var orderTotal = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

            var settings = await _settingsService.GetShopCoinSettingsAsync();

            int maxCoinsToSpend = await CalculateMaxCoinsToSpend(order.CustomerId, orderTotal);

            if (maxCoinsToSpend <= 0)
                return;

            var moneyDiscount = await ConvertFromCoinsToMoney(maxCoinsToSpend);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                wallet.Balance -= maxCoinsToSpend;
                order.TotalAmount -= moneyDiscount;

                if (order.TotalAmount < 0)
                    order.TotalAmount = 0;

                await _context.ShopCoinTransactions.AddAsync(new ShopCoinTransactionHistory
                {
                    CustomerId = order.CustomerId,
                    Coins = -maxCoinsToSpend,
                    Type = ShopCoinTransactionType.SpendOrder.ToString(),
                    OrderId = order.Id,
                    Description = $"Coins spent on order #{order.Code}"
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var message = $"You spent {maxCoinsToSpend} coins for the order #{order.Code}!";
                await _notificationService.AddNotificationForCustomerAsync(order.CustomerId, message);


                await ChangeCustomerPoints(order.CustomerId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RefundCoinsForOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            var wallet = await _context.ShopCoins
                .FirstOrDefaultAsync(w => w.CustomerId == order.CustomerId);

            if (wallet == null)
                throw new Exception("Wallet not found");

            var spentTransactions = await _context.ShopCoinTransactions
                .Where(t =>
                    t.OrderId == orderId &&
                    t.Type == ShopCoinTransactionType.SpendOrder.ToString())
                .ToListAsync();

            if (!spentTransactions.Any())
                return;

            var alreadyRefunded = await _context.ShopCoinTransactions
            .AnyAsync(t =>
                t.OrderId == orderId &&
                t.Type == ShopCoinTransactionType.Refund.ToString());

            if (alreadyRefunded)
                throw new Exception("Coins already refunded for this order");

            int coinsToRefund = spentTransactions.Sum(t => Math.Abs(t.Coins));

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                wallet.Balance += coinsToRefund;

                await _context.ShopCoinTransactions.AddAsync(new ShopCoinTransactionHistory
                {
                    CustomerId = order.CustomerId,
                    Coins = coinsToRefund,
                    Type = ShopCoinTransactionType.Refund.ToString(),
                    OrderId = order.Id,
                    Description = $"Coins refunded for cancelled order #{order.Code}"
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var message = $"You got {coinsToRefund} coins refunded for the order #{order.Code}!";
                await _notificationService.AddNotificationForCustomerAsync(order.CustomerId, message);

                await ChangeCustomerPoints(order.CustomerId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IList<ShopCoinTransactionHistoryDto>> GetAllCoinsHistory(string userId)
        {
            var customerId = await GetCustomerIdAsync(userId);

            return await _context.ShopCoinTransactions
                .Where(t => t.CustomerId == customerId)
                .Select(t => new ShopCoinTransactionHistoryDto
                {
                    Id = t.Id,
                    Coins = t.Coins,
                    Type = t.Type,
                    CustomerId = t.CustomerId,
                    OrderId = t.OrderId,
                    Description = t.Description,
                    DateCreated = t.DateCreated,
                    OrderCode = t.Order != null ? t.Order.Code : string.Empty
                })
                .OrderByDescending(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<int> GetCustomerBalance(string userId)
        {
            var customerId = await GetCustomerIdAsync(userId);

            var wallet = await _context.ShopCoins.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            return wallet?.Balance ?? 0;
        }
    }
}
