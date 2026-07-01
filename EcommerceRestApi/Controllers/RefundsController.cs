using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RefundsController : ControllerBase
    {
        private readonly IRefundService _refundService;
        private readonly ILogger<RefundsController> _logger;

        public RefundsController(IRefundService refundService, ILogger<RefundsController> logger)
        {
            _refundService = refundService;
            _logger = logger;
        }

        private bool IsAuthorizedForRefund(RefundDto refund)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            var isAdmin = User.IsInRole("Admin");
            var hasManagePermission = User.HasClaim("Permission", Permissions.ManageRefunds);

            var isOwner = customerIdClaim != null &&
                          int.TryParse(customerIdClaim, out var customerId) &&
                          customerId == refund.CustomerId;

            return isOwner || isAdmin || hasManagePermission;
        }

        private bool CanGetRefunds(int customerId)
        {
            var customerClaim = User.FindFirstValue("CustomerId");

            var isOwner = int.TryParse(customerClaim, out var userCustomerId)
                          && userCustomerId == customerId;
            Debug.WriteLine($"{customerId} {userCustomerId} {isOwner}");
            _logger.LogDebug($"{customerClaim} {userCustomerId}");
            return isOwner;
        }

        /// <summary>
        /// Get all active refunds for a specific customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>List of refunds</returns>
        [HttpGet("customer/{customerId}")]
        //[Authorize]
        [ProducesResponseType(typeof(List<RefundDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveRefunds(int customerId)
        {
            //if (!CanGetRefunds(customerId))
            //{
            //    return Forbid();
            //}

            try
            {
                var refunds = await _refundService.GetActiveRefunds(customerId);
                return Ok(refunds);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid customer ID: {CustomerId}", customerId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active refunds for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "An error occurred while retrieving refunds" });
            }
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
                var filtered = await _refundService.FilterRefundsAsync(
                    searchString, searchProperty, sortProperty, active, sortAscending);
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

        [HttpGet("order/{code}")]
        //[Authorize]
        [ProducesResponseType(typeof(List<RefundDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveRefundsForOrder(string code)
        {
            //if (!CanGetRefunds(customerId))
            //{
            //    return Forbid();
            //}

            try
            {
                var refunds = await _refundService.GetAllRefundsForOrder(code);
                return Ok(refunds);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while retrieving refunds {ex.Message}" });
            }
        }

        /// <summary>
        /// Get all refunds (Admin/Staff only)
        /// </summary>
        /// <returns>List of all refunds</returns>
        [HttpGet("all")]
        [Authorize(Policy = Permissions.ManageRefunds)]
        [ProducesResponseType(typeof(List<RefundDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRefunds()
        {
            try
            {
                var refunds = await _refundService.GetAllRefunds();
                return Ok(refunds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all refunds");
                return StatusCode(500, new { message = "An error occurred while retrieving refunds" });
            }
        }

        /// <summary>
        /// Get a specific refund by its code
        /// </summary>
        /// <param name="code">The refund code</param>
        /// <returns>The refund details</returns>
        [HttpGet("{code}")]
        [Authorize]
        [ProducesResponseType(typeof(RefundDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRefundByCode(string code)
        {
            try
            {
                var refund = await _refundService.GetRefundByCode(code);
                //if (!IsAuthorizedForRefund(refund))
                //{
                //    return Forbid();
                //}
                return Ok(refund);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid refund code: {Code}", code);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                _logger.LogWarning(ex, "Refund not found: {Code}", code);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refund {Code}", code);
                return StatusCode(500, new { message = "An error occurred while retrieving the refund" });
            }
        }

        /// <summary>
        /// Apply for a refund for specific order items
        /// </summary>
        /// <param name="request">The refund request containing order code and item IDs</param>
        /// <returns>Result of the operation</returns>
        [HttpPost("apply")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApplyForRefund([FromBody] CreateRefundDto request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.OrderCode) || request.OrderItems.Count <= 0 || !request.OrderItems.Any())
                {
                    return BadRequest(new { message = "Order code and at least one order item are required" });
                }

                await _refundService.ApplyForRefund(request.OrderCode, request, User.FindFirstValue(ClaimTypes.NameIdentifier));

                _logger.LogInformation("Refund applied for order {OrderCode} by user {UserId}",
                    request.OrderCode, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                return Ok(new
                {
                    success = true,
                    message = "Refund request submitted successfully. It will be reviewed by our team."
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid refund request for order {OrderCode}", request?.OrderCode);
                return BadRequest(new ResponseModel { Message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                _logger.LogWarning(ex, "Order not found for refund: {OrderCode}", request?.OrderCode);
                return NotFound(new ResponseModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying for refund for order {OrderCode}", request?.OrderCode);
                return StatusCode(500, new ResponseModel { Message = ex.Message });
            }
        }

        /// <summary>
        /// Change the status of a refund (Admin/Staff only)
        /// </summary>
        /// <param name="request">The status change request</param>
        /// <returns>Result of the operation</returns>
        [HttpPost("change-refund-status")]
        [Authorize(Policy = Permissions.ManageRefunds)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeRefundStatus([FromBody] ChangeRefundStatusRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.RefundCode) || string.IsNullOrEmpty(request.Status))
                {
                    return BadRequest(new { message = "Refund code and status are required" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var dto = new ChangeRefundStatusDto
                {
                    RefundCode = request.RefundCode,
                    Status = request.Status,
                };

                await _refundService.ChangeRefundStatus(dto, userId);

                _logger.LogInformation("Refund {RefundCode} status changed to {Status} by {User}",
                    request.RefundCode, request.Status, userId);

                return Ok(new
                {
                    success = true,
                    message = $"Refund status updated to {request.Status} successfully"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid status change request for refund {RefundCode}", request?.RefundCode);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                _logger.LogWarning(ex, "Refund not found for status change: {RefundCode}", request?.RefundCode);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("status transition"))
            {
                _logger.LogWarning(ex, "Invalid status transition for refund {RefundCode}", request?.RefundCode);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing refund status for {RefundCode}", request?.RefundCode);
                return StatusCode(500, new { message = "An error occurred while updating the refund status" });
            }
        }

        /// <summary>
        /// Cancel a refund request
        /// </summary>
        /// <param name="code">The refund code to cancel</param>
        /// <returns>Result of the operation</returns>
        [HttpDelete("{code}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelRefund(string code)
        {

            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest(new { message = "Refund code is required" });
                }

                var refund = await _refundService.GetRefundByCode(code);
                //if (!IsAuthorizedForRefund(refund))
                //{
                //    return Forbid();
                //}

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var currentUserIsStuffOrAdmin = userRole == "Admin" || userRole == "Staff";

                await _refundService.CancelRefund(code, currentUserIsStuffOrAdmin, User.FindFirstValue(ClaimTypes.NameIdentifier));

                _logger.LogInformation("Refund {Code} cancelled by {User}",
                    code, User.Identity?.Name ?? "System");

                return Ok(new
                {
                    success = true,
                    message = "Refund cancelled successfully"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid refund code for cancellation: {Code}", code);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                _logger.LogWarning(ex, "Refund not found for cancellation: {Code}", code);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot cancel refund"))
            {
                _logger.LogWarning(ex, "Cannot cancel refund {Code}: {Message}", code, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling refund {Code}", code);
                return StatusCode(500, new { message = $"An error occurred while cancelling the refund {ex.Message}" });
            }
        }

        [HttpGet("search-combo-box-dtos")]
        public IActionResult GetSearchComboBoxDtos()
        {
            var dtos = _refundService.GetSearchComboBoxDtos();
            return Ok(dtos);
        }

        [HttpGet("order-by-combo-box-dtos")]
        public IActionResult GetOrderByComboBoxDtos()
        {
            var dtos = _refundService.GetOrderByComboBoxDtos();
            return Ok(dtos);
        }

    }

    #region Request DTOs


    public class ChangeRefundStatusRequest
    {
        public string RefundCode { get; set; }
        public string Status { get; set; }
    }

    #endregion
}