using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShopCoinsController : ControllerBase
    {
        private readonly IShopCoinsService _service;

        public ShopCoinsController(IShopCoinsService service)
        {
            _service = service;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var userId = GetUserId();
                var history = await _service.GetAllCoinsHistory(userId);

                return Ok(history);
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel { Message = e.Message });
            }
        }

        [HttpPost("reward/{orderId}")]
        public async Task<IActionResult> Reward(int orderId)
        {
            await _service.RewardCoinsForOrder(orderId);
            return Ok(new { message = "Coins rewarded" });
        }

        [HttpPost("spend/{orderId}")]
        public async Task<IActionResult> Spend(int orderId)
        {
            await _service.SpendCoinsForOrder(orderId);
            return Ok(new { message = "Coins applied to order" });
        }

        [HttpPost("refund/{orderId}")]
        public async Task<IActionResult> Refund(int orderId)
        {
            await _service.RefundCoinsForOrder(orderId);
            return Ok(new { message = "Coins refunded" });
        }

        [HttpGet("calculate")]
        public async Task<IActionResult> CalculateCoins([FromQuery] decimal cartTotal)
        {
            var coins = await _service.ConvertMoneyToCoins(cartTotal);

            return Ok(new
            {
                cartTotal,
                coinsEarned = coins
            });
        }

        [HttpGet("max-spend/{customerId}")]
        public async Task<IActionResult> CalculateMaxSpend(int customerId, [FromQuery] decimal cartTotal)
        {
            var maxCoins = await _service.CalculateMaxCoinsToSpend(customerId, cartTotal);

            return Ok(new
            {
                cartTotal,
                maxCoinsUsable = maxCoins
            });
        }

        [HttpGet("balance/")]
        public async Task<IActionResult> GetCustomerCoinsBalance()
        {
            try
            {
                var balance = await _service.GetCustomerBalance(GetUserId());
                return Ok(balance);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}