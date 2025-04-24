using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models.Context;
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

            var model = new OrderViewModel();
            model.Customer = new CustomerViewModel();
            var customer = _userManager.Users
                                            .Include(u => u.Customers)
                                                .ThenInclude(c => c.Addresses)
                                            .FirstOrDefault(u => u.Id == _userManager.GetUserId(User))
                                            ?.Customers.FirstOrDefault();

            if (customer != null)
            {
                model.CustomerId = customer.Id;
                model.Customer = CustomerViewModel.ToVM(customer, _userManager);
            }

            model.Code = Guid.NewGuid().ToString();
            model.OrderDate = DateTime.Now;

            var cartItems = await _cart.GetCartItems();
            var orderItems = new List<OrderItemViewModel>();
            foreach (var item in cartItems)
            {
                var vm = await OrderItemViewModel.ToOrderItemVM(item, _context);
                orderItems.Add(vm);
            }

            model.OrderItems = orderItems;

            model.TotalAmount = await _cart.GetTotal();
            model.OrderDate = DateTime.Now;

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

            if (User.Identity.IsAuthenticated)
            {
                var customer = (await _userManager.Users
                                            .Include(u => u.Customers)
                                            .FirstOrDefaultAsync(u =>
                                            u.Id == _userManager.GetUserId(User)))
                                            ?.Customers.FirstOrDefault();

                model.Customer = CustomerViewModel.ToVM(customer,
                                                        _userManager);

                model.CustomerId = customer?.Id;
            }
            await _orderService.AddNewOrderAsync(model);

            return CreatedAtAction(nameof(GetOrder), new { code = model.Code }, model);
        }

        [HttpPut("update/{code}")]
        [Authorize]
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

