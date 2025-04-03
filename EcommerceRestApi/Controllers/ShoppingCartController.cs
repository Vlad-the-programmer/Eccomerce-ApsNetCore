using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using EcommerceRestApi.Models.Context;
using Microsoft.EntityFrameworkCore;
using EcommerceRestApi.Models;
using EcommerceRestApi.Helpers.Data.ResponseModels;

namespace EcommerceRestApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _context;
        private ISession _session;

        public string ShoppingCartId { get; set; }

        public ShoppingCartController(IServiceProvider services, AppDbContext context)
        {
            _serviceProvider = services;
            _context = context;
            _session = _serviceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var context = _serviceProvider.GetService<AppDbContext>();

            ShoppingCartId = _session.GetString("CartId") ?? Guid.NewGuid().ToString();
            _session.SetString("CartId", ShoppingCartId);
            return Ok(ShoppingCartId);
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetCartItems()
        {
            ShoppingCartId = _session.GetString("CartId") ?? string.Empty;

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

        [HttpPost]
        public async Task<IActionResult> AddItemToCart(Product product)
        {
            if (product == null)
            {
                return BadRequest(new ResponseModel { Message = "Invalid product!" });
            }

            try
            {
                var existingCartItem = _context.ShoppingCartItems
                                               .FirstOrDefault(n => n.Product.Id == product.Id
                                                                 && n.ShoppingCartId == ShoppingCartId);

                if (existingCartItem != null)
                {
                    // Product exists in the cart, increase the quantity
                    existingCartItem.Amount++;
                }
                else
                {
                    // Add a new item to the cart
                    var shoppingCartItem = new ShoppingCartItem
                    {
                        ShoppingCartId = ShoppingCartId,
                        ProductId = product.Id, // Avoid direct object assignment
                        Amount = 1
                    };

                    _context.ShoppingCartItems.Add(shoppingCartItem);
                }

                await _context.SaveChangesAsync(); // Use async saving
                return Ok();
            }
            catch (Exception ex)
            {
                var errorsList = new List<string>();
                errorsList.Add(ex.Message);
                return StatusCode(500, new ResponseModel { Message = "An error occurred!", Errors = errorsList });
            }
        }

        [HttpPost("remove-item/{id}")]
        public async Task<IActionResult> RemoveItemFromCart(ShoppingCartItemVM cartItem)
        {
            ShoppingCartId = _session.GetString("CartId") ?? string.Empty;

            if (ShoppingCartId == string.Empty)
            {
                return NotFound(new ResponseModel { Message = "Cart does not exist!" });
            }

            bool isCartEmpty = !_context.ShoppingCartItems.Any(n => n.ShoppingCartId == ShoppingCartId);
            if (isCartEmpty)
            {
                return BadRequest(new ResponseModel { Message = "Your cart is empty." });
            }

            try
            {
                var ShoppingCartItem = _context.ShoppingCartItems
                                                    .FirstOrDefault(n => n.Product.Id == cartItem.Id
                                                                    && n.ShoppingCartId == ShoppingCartId);
                if (ShoppingCartItem != null)
                {
                    if (ShoppingCartItem.Amount > 1)
                    {
                        ShoppingCartItem.Amount--;
                    }
                    else
                    {
                        _context.ShoppingCartItems.Remove(ShoppingCartItem);
                    }

                }
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                var errorsList = new List<string>();
                errorsList.Add(ex.Message);
                return StatusCode(500, new ResponseModel { Message = "An error occurred!", Errors = errorsList });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            string cratId = _session.GetString("CartId");
            if (string.IsNullOrEmpty(cratId))
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
