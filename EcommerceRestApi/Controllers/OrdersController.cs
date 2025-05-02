using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;
        private readonly AppDbContext _context;
        private readonly ShoppingCart _cart;

        public OrdersController(UserManager<ApplicationUser> userManager,
                                IOrderService orderService,
                                AppDbContext context,
                                ShoppingCart cart)
        {
            _userManager = userManager;
            _orderService = orderService;
            _context = context;
            _cart = cart;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{code}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(string code)
        {
            var order = await _orderService.GetOrderByCodeAsync(code);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("get-order-create-model")]
        //[Authorize]
        public async Task<IActionResult> CreateOrderCreateTemplate()
        {

            var model = new NewOrderViewModel();
            model.Customer = new CreateOrderCustomerDto();

            var customer = await _context.Customers
                                            .Include(c => c.User)
                                            .Include(c => c.Addresses)
                                                .ThenInclude(a => a.Country)
                                            .Where(c => c.UserId == _userManager.GetUserId(User))
                                            .FirstOrDefaultAsync();
            if (customer != null)
            {
                model.Customer = CreateOrderCustomerDto.ToDto(customer, _userManager);
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList()
                });
            }

            return Ok(model);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] NewOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList()
                });
            }

            var order = await _orderService.AddNewOrderAsync(model);

            return CreatedAtAction(nameof(GetOrder), new { code = order.Code }, order);
        }

        [HttpPut("update/{code}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder(string code, [FromBody] NewOrderViewModel model)
        {
            var order = await _orderService.GetOrderByCodeAsync(code);
            if (order == null)
                return NotFound();

            await _orderService.UpdateOrderAsync(code, model);

            return NoContent();
        }

        [HttpDelete("delete/{code}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(string code)
        {
            var order = await _orderService.GetOrderByCodeAsync(code);
            if (order == null)
                return NotFound();

            await _orderService.DeleteOrderAsync(code);

            return NoContent();
        }
    }
}

