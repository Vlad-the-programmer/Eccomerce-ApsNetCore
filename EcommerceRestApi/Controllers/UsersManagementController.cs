using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
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
    public class UsersManagementController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersManagementService _usersManagerService;

        public UsersManagementController(UserManager<ApplicationUser> userManager,
                                        IUserService userService,
                                        IUsersManagementService usersManagementService)
        {
            _userManager = userManager;
            _usersManagerService = usersManagementService;
        }

        [HttpGet("get-staff-users")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetStaffUsers()
        {
            var users = await _usersManagerService.GetStaffUsers();
            return Ok(users);
        }

        [HttpGet("get-update-user-model/{id}")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Stuff}")]
        public async Task<IActionResult> GetUpdateUserModel(string id)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found ");
            }

            var staffUpdateVM = await _usersManagerService.GetUpdateUserModel(user);

            return Ok(staffUpdateVM);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Stuff}")]
        public async Task<IActionResult> Update(string id, [FromBody] StaffUpdateVM model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid model data",
                    Errors = ModelState.Values
                                             .SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList()
                });
            }

            var user = await _userManager.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            await _usersManagerService.UpdateStaffAsync(user, model, user.Role);
            return NoContent();
        }

        [HttpGet("{userId}/permissions")]
        [Authorize(Policy = Permissions.ManageUsers)]
        public async Task<IActionResult> GetPermissions(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var pemissions = await _usersManagerService.GetPermissions(user);

            return Ok(pemissions);
        }

        [HttpPost("bulk-update-permissions")]
        [Authorize(Policy = Permissions.ManageUsers)]
        public async Task<IActionResult> BulkUpdatePermissions([FromBody] UpdatePermissionsDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            await _usersManagerService.BulkUpdatePerms(dto, user);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.ManageUsers)]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            await _usersManagerService.DeleteStaffUser(user);
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = Permissions.ManageUsers)]
        public async Task<IActionResult> CreateStaffUser([FromBody] CreateStaffVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid model data",
                    Errors = ModelState.Values
                                             .SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList()
                });
            }
            var result = await _usersManagerService.CreateStaff(model);

            return Ok(result);
        }

        [HttpGet("filter-staff")]
        [Authorize(Policy = Permissions.ManageUsers)]
        public async Task<IActionResult> Filter(
               [FromQuery] string searchString = "",
               [FromQuery] string? searchProperty = null,
               [FromQuery] string? sortProperty = null,
               [FromQuery] string? statusFilter = null,
               [FromQuery] bool sortAscending = false)
        {
            try
            {
                var filteredStaff = await _usersManagerService.FilterStaffAsync(
                    searchString, searchProperty, sortProperty, statusFilter, sortAscending);
                return Ok(filteredStaff);
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

        [HttpGet("search-combo-box-dtos")]
        public IActionResult GetSearchComboBoxDtos()
        {
            var dtos = _usersManagerService.GetSearchComboBoxDtos();
            return Ok(dtos);
        }

        [HttpGet("order-by-combo-box-dtos")]
        public IActionResult GetOrderByComboBoxDtos()
        {
            var dtos = _usersManagerService.GetOrderByComboBoxDtos();
            return Ok(dtos);
        }

        public class UpdatePermissionsDto
        {
            public string UserId { get; set; }
            public List<string> Permissions { get; set; } = new();
        }
    }
}
