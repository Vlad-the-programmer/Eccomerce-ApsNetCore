using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
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
        //[Authorize]
        public async Task<IActionResult> GetUserOrders(int customerId)
        {
            var orders = await _orderService.GetUserOrdersAsync(customerId);
            return Ok(orders);
        }

        [HttpGet("{code}")]
        //[Authorize]
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

            var model = await _orderService.CreateOrderCreateTemplate(shoppingCartId, User);
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

            try
            {
                var order = await _orderService.AddNewOrderAsync(model);

                return CreatedAtAction(nameof(GetOrder), new { code = order.Code }, order);
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel { Message = e.Message });
            }
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
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteOrder(string code)
        {
            var order = await _orderService.GetOrderByCodeAsync(code);
            if (order == null)
                return NotFound();

            try
            {

                await _orderService.DeleteOrderAsync(code);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        [HttpPost("change-order-status")]
        [Authorize(Policy = Permissions.ManageOrders)]
        public async Task<IActionResult> ChangeOrderStatus([FromBody] ChangeOrderStatusDto changeOrderStatusDto)
        {
            try
            {
                await _orderService.ChangeOrderStatusAsync(changeOrderStatusDto, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel { Message = e.Message });
            }
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

