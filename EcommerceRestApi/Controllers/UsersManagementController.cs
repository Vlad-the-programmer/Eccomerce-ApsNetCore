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
    [Authorize(Roles = UserRoles.Admin)]
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
        public async Task<IActionResult> GetStaffUsers()
        {
            var users = await _usersManagerService.GetStaffUsers();
            return Ok(users);
        }

        [HttpGet("get-update-user-model/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        [Authorize(Policy = Permissions.ManageUsers)]
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
        [Authorize(Policy = Permissions.ManageUsers)]
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

            await _usersManagerService.UpdateStaffAsync(user, model);
            return NoContent();
        }

        [HttpGet("{userId}/permissions")]
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

        public class UpdatePermissionsDto
        {
            public string UserId { get; set; }
            public List<string> Permissions { get; set; } = new();
        }
    }
}
