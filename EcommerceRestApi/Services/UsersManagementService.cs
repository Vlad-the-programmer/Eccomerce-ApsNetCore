using EcommerceRestApi.Helpers.Data.AuthVms;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
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
        public UsersManagementService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            return new ResponseModel { Message = "Staff member created successfully." };
        }

        public async Task UpdateStaffAsync(ApplicationUser updatedUser, StaffUpdateVM userUpdateVM)
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

                if (userUpdateVM.IsActive)
                {
                    updatedUser.IsActive = true;
                    updatedUser.DateDeleted = null;
                }
                else
                {
                    updatedUser.IsActive = false;
                }

                await _context.SaveChangesAsync();
            }
        }

    }
}
