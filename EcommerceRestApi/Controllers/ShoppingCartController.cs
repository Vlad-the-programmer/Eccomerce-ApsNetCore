using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using EcommerceRestApi.Models.Context;
using Microsoft.EntityFrameworkCore;
using EcommerceRestApi.Models;
using EcommerceRestApi.Helpers.Data.ResponseModels;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _context;
        private ISession session;

        public string ShoppingCartId { get; set; }

        public ShoppingCartController(IServiceProvider services, AppDbContext context)
        {
            _serviceProvider = services;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            session = _serviceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = _serviceProvider.GetService<AppDbContext>();

            ShoppingCartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", ShoppingCartId);
            return Ok(ShoppingCartId);
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
                return Ok(new ResponseModel { Message = "Item added to cart successfully!" });
            }
            catch (Exception ex)
            {
                var errorsList = new List<string>();
                errorsList.Add(ex.Message);
                return StatusCode(500, new ResponseModel { Message = "An error occurred!", Errors = errorsList });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveItemFromCart(Product product)
        {
            if (product == null)
            {
                return BadRequest(new ResponseModel { Message = "Invalid product!" });
            }

            bool isCartEmpty = !_context.ShoppingCartItems.Any(n => n.ShoppingCartId == ShoppingCartId);
            if (isCartEmpty)
            {
                return BadRequest(new ResponseModel { Message = "Your cart is empty." });
            }

            try
            {
                var ShoppingCartItem = _context.ShoppingCartItems
                                                    .FirstOrDefault(n => n.Product.Id == product.Id
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

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            string cratId = session.GetString("CartId");
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
