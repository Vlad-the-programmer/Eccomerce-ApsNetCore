using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReturnController : ControllerBase
    {
        private readonly IReturnService _returnService;
        private readonly ILogger<ReturnController> _logger;

        public ReturnController(IReturnService returnService, ILogger<ReturnController> logger)
        {
            _returnService = returnService;
            _logger = logger;
        }

        private bool IsOwnerOrAdmin(ReturnDto returnDto)
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;
            var isAdmin = User.IsInRole(UserRoles.Admin);
            var hasPermission = User.HasClaim("Permission", Permissions.ManageRefunds);

            var isOwner = customerIdClaim != null &&
                          int.TryParse(customerIdClaim, out var customerId);

            return isOwner || isAdmin || hasPermission;
        }

        private bool CanAccessCustomer(int customerId)
        {
            var claim = User.FindFirst("CustomerId")?.Value;
            var isAdmin = User.IsInRole(UserRoles.Admin);
            var hasPermission = User.HasClaim("Permission", Permissions.ManageRefunds);

            return isAdmin || hasPermission || (claim != null && int.Parse(claim) == customerId);
        }

        [HttpGet("all")]
        [Authorize(Policy = Permissions.ManageRefunds)]
        public async Task<IActionResult> GetAll()
        {
            var returns = await _returnService.GetAllReturnsAsync();
            return Ok(returns);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerReturns(int customerId)
        {
            if (!CanAccessCustomer(customerId))
                return Forbid();

            var returns = await _returnService.GetCustomerReturnsAsync(customerId);
            return Ok(returns);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
                [FromQuery] string searchString = "",
                [FromQuery] string? searchProperty = null,
                [FromQuery] string? sortProperty = null,
                [FromQuery] bool active = false,
                [FromQuery] bool sortAscending = false)
        {
            try
            {
                var filtered = await _returnService.FilterReturnsAsync(
                    searchString, searchProperty, sortProperty, sortAscending);
                return Ok(filtered);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    Message = "An error occurred while filtering refunds.",
                    Errors = new List<string>().Append(ex.Message).ToList()
                });
            }

        }

        [HttpGet("{refundCode}")]
        public async Task<IActionResult> GetByRefundCode(string refundCode)
        {
            if (string.IsNullOrWhiteSpace(refundCode) || refundCode == "?")
            {
                return BadRequest(new ResponseModel { Message = "Invalid refund code received" });
            }

            var result = await _returnService.GetReturnByRefundCode(refundCode);

            if (result == null)
                return NotFound(new { message = "Return not found" });

            if (!IsOwnerOrAdmin(result))
                return Forbid();

            return Ok(result);
        }

        //[HttpPost("create/{refundCode}")]
        //public async Task<IActionResult> Create(string refundCode)
        //{
        //    try
        //    {
        //        await _returnService.CreateReturn(refundCode);

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Return created successfully"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning(ex, "Error creating return for {RefundCode}", refundCode);
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        [HttpPost("change-status")]
        [Authorize(Policy = Permissions.ManageRefunds)]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeReturnStatusDto dto)
        {
            try
            {
                await _returnService.ChangeReturnStatus(dto);

                return Ok(new
                {
                    success = true,
                    message = "Return status updated"
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error changing return status");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{refundCode}")]
        public async Task<IActionResult> Cancel(string refundCode)
        {
            try
            {
                var existing = await _returnService.GetReturnByRefundCode(refundCode);

                if (existing == null)
                    return NotFound(new { message = "Return not found" });

                if (!IsOwnerOrAdmin(existing))
                    return Forbid();

                await _returnService.CancelReturn(refundCode);

                return Ok(new
                {
                    success = true,
                    message = "Return cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error cancelling return");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search-combo-box-dtos")]
        public IActionResult GetSearchComboBoxDtos()
        {
            var dtos = _returnService.GetSearchComboBoxDtos();
            return Ok(dtos);
        }

        [HttpGet("order-by-combo-box-dtos")]
        public IActionResult GetOrderByComboBoxDtos()
        {
            var dtos = _returnService.GetOrderByComboBoxDtos();
            return Ok(dtos);
        }
    }
}