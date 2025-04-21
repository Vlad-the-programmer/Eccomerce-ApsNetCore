using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommerceRestApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly ISession? _session;
        private readonly ShoppingCart _cart;

        public string ShoppingCartId { get; set; }

        public ShoppingCartController(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _session = _httpContextAccessor?.HttpContext?.Session;
            _cart = new ShoppingCart(_context, _session).GetShoppingCart();
            ShoppingCartId = _cart.IdCartSession;
            Debug.WriteLine($"Shopping cart id: {ShoppingCartId}");
        }

        [HttpGet]
        public async Task<IActionResult> GetCreateCart()
        {
            var cartVM = new ShoppingCartVM
            {
                ShoppingCartItems = await _cart.GetCartItems(),
                CartTotal = (double)await _cart.GetTotal()
            };
            return Ok(cartVM);
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetCartItems()
        {

            if (ShoppingCartId == string.Empty)
            {
                return NotFound(new ResponseModel { Message = "Cart does not exist!" });
            }

            bool isCartEmpty = !_context.ShoppingCartItems.Any(n => n.ShoppingCartId == ShoppingCartId);
            if (isCartEmpty)
            {
                return BadRequest(new ResponseModel { Message = "Your cart is empty." });
            }

            var ShoppingCartItems = _context.ShoppingCartItems
                                                     .Where(n => n.ShoppingCartId == ShoppingCartId).ToList();

            return Ok(ShoppingCartItems);
        }

        [HttpGet("cart-item/{productId}")]
        public async Task<IActionResult> GetCartItem(int productId)
        {
            if (await _context.Products.FindAsync(productId) == null)
            {
                return NotFound();
            }

            if (ShoppingCartId == string.Empty)
            {
                return NotFound(new ResponseModel { Message = "Cart does not exist!" });
            }

            var shoppingCartItem = _context.ShoppingCartItems
                                                     .FirstOrDefault(n => n.ShoppingCartId == ShoppingCartId
                                                                        && n.ProductId == productId);

            if (shoppingCartItem == null)
            {
                return NotFound();
            }
            return Ok(shoppingCartItem);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToCart(int productId)
        {
            if (await _context.Products.FindAsync(productId) == null)
            {
                return NotFound();
            }

            await _cart.AddToCartHandler(productId);
            return Ok();
        }

        [HttpPost("remove-item/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            if (await _context.Products.FindAsync(productId) == null)
            {
                return NotFound();
            }

            if (ShoppingCartId == string.Empty)
            {
                return NotFound(new ResponseModel { Message = "Cart does not exist!" });
            }

            bool isCartEmpty = !_context.ShoppingCartItems.Any(n => n.ShoppingCartId == ShoppingCartId);
            if (isCartEmpty)
            {
                return BadRequest(new ResponseModel { Message = "Your cart is empty." });
            }


            await _cart.DeleteFromCartHandler(productId);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            if (string.IsNullOrEmpty(ShoppingCartId))
            {
                return NotFound(new ResponseModel { Message = "Cart does not exist" });
            }
            var items = await _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToListAsync();
            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
