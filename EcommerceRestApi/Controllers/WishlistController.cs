using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishListService _service;

        public WishlistController(IWishListService service)
        {
            _service = service;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWishlist()
        {
            try
            {
                var userId = GetUserId();

                var wishlist = await _service.CreateWishListAsync(userId);

                return Ok(wishlist);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("add-item/{productId}")]
        public async Task<IActionResult> AddItem(int productId)
        {
            try
            {
                var userId = GetUserId();

                await _service.AddWishlistItem(productId, userId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(new { message = "Item added to wishlist" });
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            try
            {
                var userId = GetUserId();

                var wishlist = await _service.GetWishlistByUserId(userId);
                return Ok(wishlist);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetItems()
        {
            try
            {
                var userId = GetUserId();
                var items = await _service.GetWishlistItemsByUserIdAsync(userId);

                return Ok(items);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            try
            {
                var userId = GetUserId();
                await _service.RemoveWishlistItemAsync(id, userId);

                return Ok(new { message = "Item removed from wishlist" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("transfer-to-cart")]
        public async Task<IActionResult> TransferToCart()
        {
            try
            {
                var userId = GetUserId();
                await _service.TransferWishlistToCartAsync(userId);
                return Ok(new { message = "Wishlist items transferred to cart" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            try
            {
                var userId = GetUserId();
                await _service.ClearWishlistAsync(userId);
                return Ok(new { message = "Wishlist cleared" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}