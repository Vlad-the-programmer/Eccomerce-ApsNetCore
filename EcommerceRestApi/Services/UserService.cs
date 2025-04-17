using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Helpers.Static;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using Microsoft.EntityFrameworkCore;


namespace EcommerceRestApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }
        public async Task DeleteUserAsync(string id)
        {
            ApplicationUser? user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.IsActive = false;
                user.IsAdmin = false;
                user.DateDeleted = DateTime.Now;

                user.Customers.First().IsActive = false;
                user.Customers.First().DateDeleted = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIDAsync(string id)
        {
            return await _context.Users
                .Include(u => u.Customers)
                .ThenInclude(c => c.Orders)
                .ThenInclude(o => o.Payments)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateUserAsync(string id, UserUpdateVM userUpdateVM)
        {
            ApplicationUser? updatedUser = _context.Users.FirstOrDefault(u => u.Id == id);

            if (updatedUser != null)
            {
                updatedUser.UserName = userUpdateVM.Username ?? updatedUser.UserName;
                updatedUser.Email = userUpdateVM.Email ?? updatedUser.Email;
                updatedUser.FirstName = userUpdateVM.FirstName ?? updatedUser.FirstName;
                updatedUser.LastName = userUpdateVM.LastName ?? updatedUser.LastName;
                updatedUser.FullName = updatedUser.FirstName + " " + updatedUser.LastName;
                updatedUser.PhoneNumber = userUpdateVM.PhoneNumber ?? updatedUser.PhoneNumber;
                updatedUser.IsActive = userUpdateVM.IsActive != updatedUser.IsActive ? userUpdateVM.IsActive : updatedUser.IsActive;
                updatedUser.IsAdmin = userUpdateVM.IsAdmin != updatedUser.IsAdmin ? userUpdateVM.IsAdmin : updatedUser.IsAdmin;
                updatedUser.Role = updatedUser.IsAdmin ? UserRoles.Admin : UserRoles.User;
                updatedUser.DateUpdated = DateTime.Now;

                updatedUser.Customers.First().IsActive = updatedUser.IsActive;
                updatedUser.Customers.First().Points = userUpdateVM.Points ?? updatedUser.Customers.First().Points;
                updatedUser.Customers.First().DateUpdated = DateTime.Now;

                Address address = updatedUser.Customers.First().Addresses.FirstOrDefault() ?? new Address();

                var country = DbFuncs.GetCountriesList(_context).FirstOrDefault(c => c.CountryName == userUpdateVM.CountryName);
                address.City = userUpdateVM.City ?? address.City;
                address.CountryId = userUpdateVM.CountryName != null ? address.CountryId : country != null ? country.Id : address.Id;
                address.FlatNumber = userUpdateVM.FlatNumber ?? address.FlatNumber;
                address.HouseNumber = userUpdateVM.HouseNumber ?? address.HouseNumber;
                address.PostalCode = userUpdateVM.PostalCode ?? address.PostalCode;
                address.IsActive = userUpdateVM.IsActive;
                address.State = userUpdateVM.State ?? address.State;
                address.Street = userUpdateVM.Street ?? address.Street;

                if (updatedUser.Customers.First().Addresses.FirstOrDefault() == null)
                {
                    updatedUser.Customers.First().Addresses.Add(address);
                }

                await _context.SaveChangesAsync();
            }
            ;
        }
    }
}
