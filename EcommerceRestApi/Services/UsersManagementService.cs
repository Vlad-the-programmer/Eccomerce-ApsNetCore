using EcommerceRestApi.Helpers.Data.AuthVms;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static EcommerceRestApi.Controllers.UsersManagementController;

namespace EcommerceRestApi.Services
{
    public class UsersManagementService : IUsersManagementService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public UsersManagementService(AppDbContext context,
                UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<IList<string>> GetPermissions(ApplicationUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);

            return claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();
        }

        public async Task BulkUpdatePerms(UpdatePermissionsDto dto, ApplicationUser user)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);

            var currentPermissions = existingClaims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var newPermissions = dto.Permissions ?? new List<string>();

            var toAdd = newPermissions.Except(currentPermissions);

            var toRemove = currentPermissions.Except(newPermissions);

            foreach (var perm in toAdd)
            {
                await _userManager.AddClaimAsync(user, new Claim("Permission", perm));
            }

            foreach (var perm in toRemove)
            {
                var claim = existingClaims.FirstOrDefault(c => c.Type == "Permission" && c.Value == perm);
                if (claim != null)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }
            }

            var addedPermissions = toAdd.Any()
                ? string.Join(", ", toAdd)
                : "none";

            var removedPermissions = toRemove.Any()
                ? string.Join(", ", toRemove)
                : "none";

            var message = $"Your Permissions got updated. Added: {addedPermissions}. Removed: {removedPermissions}.";
            await _notificationService.AddNotificationForUserAsync(user.Id, message);
        }

        public async Task<StaffUpdateVM> GetUpdateUserModel(ApplicationUser user)
        {

            return new StaffUpdateVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                IsActive = user.IsActive
            };
        }

        public async Task<List<CurrentUserViewModel>> GetStaffUsers()
        {
            return await _userManager.Users
                .Where(u => u.Role == UserRoles.Stuff)
                .Select(u => (CurrentUserViewModel)u)
                .ToListAsync();
        }

        public async Task DeleteStaffUser(ApplicationUser user)
        {
            user.IsActive = false;
            user.DateDeleted = null;

            await _context.SaveChangesAsync();
        }

        public async Task<ResponseModel> CreateStaff(CreateStaffVM newUserVM)
        {
            var user = await _userManager.FindByEmailAsync(newUserVM.Email);
            if (user != null)
            {
                return new ResponseModel { Message = "This email address is already in use." };
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                FullName = newUserVM.FirstName + " " + newUserVM.LastName,
                Email = newUserVM.Email,
                UserName = newUserVM.UserName ?? newUserVM.Email.Split("@")[0],
                FirstName = newUserVM.FirstName,
                LastName = newUserVM.LastName,
                Role = UserRoles.Stuff,
                PhoneNumber = newUserVM.PhoneNumber,
                DateCreated = DateTime.Now
            };

            newUser.IsAdmin = false;
            newUser.IsActive = true;

            var newUserResponse = await _userManager.CreateAsync(newUser, newUserVM.Password);
            if (!newUserResponse.Succeeded)
            {
                return new ResponseModel
                {
                    Message = "User registration failed.",
                    Errors = newUserResponse.Errors.Select(e => e.Description).ToList()
                };
            }

            await _userManager.AddToRoleAsync(newUser, UserRoles.Stuff);

            var message = $"Your profile got created. Welocome in our staff!";
            await _notificationService.AddNotificationForUserAsync(newUser.Id, message);

            return new ResponseModel { Message = "Staff member created successfully." };
        }

        public async Task UpdateStaffAsync(ApplicationUser updatedUser, StaffUpdateVM userUpdateVM, string currentUserRole)
        {
            if (updatedUser != null)
            {
                updatedUser.UserName = userUpdateVM.Username ?? updatedUser.UserName;
                updatedUser.Email = userUpdateVM.Email ?? updatedUser.Email;
                updatedUser.FirstName = userUpdateVM.FirstName ?? updatedUser.FirstName;
                updatedUser.LastName = userUpdateVM.LastName ?? updatedUser.LastName;
                updatedUser.FullName = $"{updatedUser.FirstName} {updatedUser.LastName}";
                updatedUser.PhoneNumber = userUpdateVM.PhoneNumber ?? updatedUser.PhoneNumber;
                updatedUser.DateUpdated = DateTime.Now;

                if (currentUserRole == UserRoles.Admin)
                {
                    if (userUpdateVM.IsActive)
                    {
                        updatedUser.IsActive = true;
                        updatedUser.DateDeleted = null;
                    }
                    else
                    {
                        updatedUser.IsActive = false;
                    }

                }

                await _context.SaveChangesAsync();
            }
        }

        public List<SearchComboBoxDto> GetSearchComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(ApplicationUser.Email),
                    DisplayName = "Email"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = "FullName",
                    DisplayName = "Full Name"
                }
            };
        }

        public List<SearchComboBoxDto> GetOrderByComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(ApplicationUser.Email),
                    DisplayName = "Email"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = "FullName",
                    DisplayName = "Full Name"
                }
            };
        }

        public async Task<List<CurrentUserViewModel>> FilterStaffAsync(
            string searchString, string? searchProperty, string? sortProperty,
                string statusFilter = "",
                bool sortAscending = false)
        {
            var filterEnum = UserStatusFilter.All;
            if (!string.IsNullOrEmpty(statusFilter))
            {
                switch (statusFilter.ToLower())
                {
                    case "active":
                        filterEnum = UserStatusFilter.ActiveOnly;
                        break;
                    case "inactive":
                        filterEnum = UserStatusFilter.InactiveOnly;
                        break;
                    case "all":
                    default:
                        filterEnum = UserStatusFilter.All;
                        break;
                }
            }

            var staff = await GetStaffUsers();

            switch (filterEnum)
            {
                case UserStatusFilter.ActiveOnly:
                    staff = staff.Where(item => item.IsActive).ToList();
                    break;
                case UserStatusFilter.InactiveOnly:
                    staff = staff.Where(item => !item.IsActive).ToList();
                    break;
                case UserStatusFilter.All:
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchProperty)
                {
                    case "Email":
                        staff = staff.Where(item => item.Email.ToLower().Contains(searchString.ToLower())).ToList();
                        break;
                    case "FullName":
                        staff = staff.Where(item => item.FullName.ToLower().Contains(searchString.ToLower())).ToList();
                        break;
                }
            }

            if (!string.IsNullOrEmpty(sortProperty))
            {
                switch (sortProperty)
                {
                    case "FullName":
                        staff = sortAscending ? staff.OrderBy(item => item.FullName).ToList() : staff.OrderByDescending(item => item.FullName).ToList();
                        break;
                    case "Email":
                        staff = sortAscending ? staff.OrderBy(item => item.Email).ToList() : staff.OrderByDescending(item => item.Email).ToList();
                        break;
                }
            }
            else
            {
                staff = staff.OrderBy(o => o.FullName).ToList();
            }

            return staff;
        }
    }
}
