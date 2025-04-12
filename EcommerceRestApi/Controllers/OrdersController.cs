using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;

        public OrdersController(UserManager<ApplicationUser> userManager, IOrderService orderService)
        {
            _userManager = userManager;
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{code}")]
        //[Authorize]
        public async Task<IActionResult> GetOrder(string code)
        {
            var order = await _orderService.GetOrderByCodeAsync(code);

            if (order == null)
                return NotFound();

            //if (order.Customer.User.Id != _userManager.GetUserId(User))
            //{
            //    return Forbid();
            //}

            return Ok(order);
        }

        [HttpPost("create")]
        //[Authorize]
        public async Task<IActionResult> CreateOrder(OrderViewModel model)
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

            model.CustomerId = _userManager.Users.First(u => u.Id == _userManager.GetUserId(User)).Customers.First().Id;
            await _orderService.AddNewOrderAsync(model);

            return CreatedAtAction(nameof(GetOrder), new { code = model.Code }, model);
        }

        [HttpPut("update/{code}")]
        //[Authorize]
        public async Task<IActionResult> UpdateOrder(string code, OrderViewModel model)
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

