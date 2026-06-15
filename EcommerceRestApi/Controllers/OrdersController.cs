using EcommerceRestApi.AppGlobals;
using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
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

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
                [FromQuery] string searchString = "",
                [FromQuery] string? searchProperty = null,
                [FromQuery] string? sortProperty = null,
                [FromQuery] bool sortAscending = false,
                [FromQuery] DateTime? fromDate = null,
                [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var filteredProducts = await _orderService.FilterOrdersAsync(
                    searchString, searchProperty, sortProperty, fromDate, toDate, sortAscending);
                return Ok(filteredProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    Message = "An error occurred while filtering products.",
                    Errors = new List<string>().Append(ex.Message).ToList()
                });
            }
        }

        [HttpGet]
        [Authorize(Policy = Permissions.ManageOrders)]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders(int customerId)
        {
            var orders = (await _orderService.GetOrdersAsync()).Where(o => o.CustomerId == customerId);
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

        [HttpGet("get-order-create-model/{shoppingCartId}")]
        public async Task<IActionResult> CreateOrderCreateTemplate(string shoppingCartId)
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

            model.TotalAmount += await _cart.GetTotal();
            //model.TotalAmount += model.TotalAmount * AppConstants.TAXES_RATE;

            model.TaxRate = AppConstants.TAXES_RATE;

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
        [Authorize(Policy = Permissions.ManageOrders)]
        public async Task<IActionResult> CreateOrder([FromBody][Bind("DeliveryMethod,   " +
            "                               PaymentMethod,OrderStatus,Customer,TotalAmount")] NewOrderViewModel model)
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
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> UpdateOrder(string code, [FromBody] NewOrderViewModel model)
        {
            var order = await _orderService.GetOrderByCodeAsync(code);
            if (order == null)
                return NotFound();

            await _orderService.UpdateOrderAsync(code, model);

            return NoContent();
        }

        [HttpDelete("delete/{code}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteOrder(string code)
        {
            var order = await _orderService.GetOrderByCodeAsync(code);
            if (order == null)
                return NotFound();

            await _orderService.DeleteOrderAsync(code);

            return NoContent();
        }

        [HttpGet("search-combo-box-dtos")]
        public IActionResult GetSearchComboBoxDtos()
        {
            var dtos = _orderService.GetSearchComboBoxDtos();
            return Ok(dtos);
        }

        [HttpGet("order-by-combo-box-dtos")]
        public IActionResult GetOrderByComboBoxDtos()
        {
            var dtos = _orderService.GetOrderByComboBoxDtos();
            return Ok(dtos);
        }
    }
}

