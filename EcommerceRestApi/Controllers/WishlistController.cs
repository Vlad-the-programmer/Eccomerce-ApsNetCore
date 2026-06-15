using EcommerceRestApi.Models.Dtos;
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
            var userId = GetUserId();

            await _service.CreateWishListAsync(userId);

            return Ok(new { message = "Wishlist created (if not exists)" });
        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItem([FromBody] WishListItemDto item)
        {
            var userId = GetUserId();

            await _service.AddWishlistItem(item, userId);

            return Ok(new { message = "Item added to wishlist" });
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetUserId();

            var wishlist = await _service.GetWishlistByUserId(userId);

            return Ok(wishlist);
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetItems()
        {
            var userId = GetUserId();

            var items = await _service.GetWishlistItemsByUserIdAsync(userId);

            return Ok(items);
        }

        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var userId = GetUserId();

            await _service.RemoveWishlistItemAsync(id, userId);

            return Ok(new { message = "Item removed from wishlist" });
        }
    }
}