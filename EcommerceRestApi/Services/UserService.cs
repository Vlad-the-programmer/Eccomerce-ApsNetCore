using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
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

                var customer = user.Customers.FirstOrDefault();
                if (customer != null)
                {
                    customer.IsActive = false;
                    customer.DateDeleted = DateTime.Now;
                }
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
                    .ThenInclude(c => c.Addresses)
                        .ThenInclude(c => c.Country)
                .Include(u => u.Customers)
                    .ThenInclude(c => c.Orders)
                .ThenInclude(o => o.Payments)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateUserAsync(string id, UserUpdateVM userUpdateVM)
        {
            ApplicationUser? updatedUser = _context.Users
                                                .Include(u => u.Customers)
                                                    .ThenInclude(c => c.Addresses)
                                                        .ThenInclude(a => a.Country)
                                                .FirstOrDefault(u => u.Id == id);

            if (updatedUser != null)
            {
                updatedUser.UserName = userUpdateVM.Username ?? updatedUser.UserName;
                updatedUser.Email = userUpdateVM.Email ?? updatedUser.Email;
                updatedUser.FirstName = userUpdateVM.FirstName ?? updatedUser.FirstName;
                updatedUser.LastName = userUpdateVM.LastName ?? updatedUser.LastName;
                updatedUser.FullName = updatedUser.FirstName + " " + updatedUser.LastName;
                updatedUser.PhoneNumber = userUpdateVM.PhoneNumber ?? updatedUser.PhoneNumber;
                updatedUser.DateUpdated = DateTime.Now;

                var customer = updatedUser.Customers.FirstOrDefault();
                if (customer != null)
                {
                    customer.Nip = userUpdateVM.Nip ?? updatedUser.Customers.FirstOrDefault()?.Nip;
                    customer.DateUpdated = DateTime.Now;

                }

                var oldAddress = updatedUser.Customers.First().Addresses.FirstOrDefault();
                Address address = oldAddress ?? new Address();

                var country = DbFuncs.GetCountriesList(_context).FirstOrDefault(c => c.CountryName == userUpdateVM.CountryName);
                address.City = userUpdateVM.City ?? address.City;
                address.CountryId = userUpdateVM.CountryName != null ? address.CountryId : country != null ? country.Id : address.Id;
                address.FlatNumber = userUpdateVM.FlatNumber ?? address.FlatNumber;
                address.HouseNumber = userUpdateVM.HouseNumber ?? address.HouseNumber;
                address.PostalCode = userUpdateVM.PostalCode ?? address.PostalCode;
                address.State = userUpdateVM.State ?? address.State;
                address.Street = userUpdateVM.Street ?? address.Street;

                if (oldAddress == null)
                {
                    address.DateCreated = DateTime.Now;
                    updatedUser.Customers.First().Addresses.Add(address);
                }
                else
                {
                    address.DateUpdated = DateTime.Now;
                    _context.Entry(address).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
            }
            ;
        }

        public async Task<UserProfileDto?> GetUserProfile(string userId)
        {
            var user = await GetUserByIDAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            return await _context.Users
                .Where(u => u.Id == userId)
                 .Include(u => u.Customers)
                    .ThenInclude(c => c.Addresses)
                        .ThenInclude(c => c.Country)
                .Select(
                u => new UserProfileDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    UserName = u.UserName,
                    PhoneNumber = u.PhoneNumber,
                    Nip = u.Customers.FirstOrDefault() != null ? u.Customers.FirstOrDefault().Nip : string.Empty,
                    Role = u.Role,
                    CoinsBalance = u.Customers.FirstOrDefault() != null ? u.Customers.FirstOrDefault().Points : 0,
                    IsActive = u.IsActive,
                    IsAdmin = u.IsAdmin,
                    IsAuthenticated = true,
                    CountryName = u.Customers.FirstOrDefault() != null && u.Customers.FirstOrDefault().Addresses.FirstOrDefault() != null && u.Customers.FirstOrDefault().Addresses.FirstOrDefault().Country != null
                                    ? u.Customers.FirstOrDefault().Addresses.FirstOrDefault().Country.CountryName
                                    : string.Empty,
                    Street = u.Customers.FirstOrDefault() != null && u.Customers.FirstOrDefault().Addresses.FirstOrDefault() != null
                                    ? u.Customers.FirstOrDefault().Addresses.FirstOrDefault().Street
                                    : string.Empty,
                    HouseNumber = u.Customers.FirstOrDefault() != null && u.Customers.FirstOrDefault().Addresses.FirstOrDefault() != null
                                    ? u.Customers.FirstOrDefault().Addresses.FirstOrDefault().HouseNumber
                                    : string.Empty,
                    FlatNumber = u.Customers.FirstOrDefault() != null && u.Customers.FirstOrDefault().Addresses.FirstOrDefault() != null
                                    ? u.Customers.FirstOrDefault().Addresses.FirstOrDefault().FlatNumber
                                    : string.Empty,
                    City = u.Customers.FirstOrDefault() != null && u.Customers.FirstOrDefault().Addresses.FirstOrDefault() != null
                                    ? u.Customers.FirstOrDefault().Addresses.FirstOrDefault().City
                                    : string.Empty,
                    State = u.Customers.FirstOrDefault() != null && u.Customers.FirstOrDefault().Addresses.FirstOrDefault() != null
                                    ? u.Customers.FirstOrDefault().Addresses.FirstOrDefault().State
                                    : string.Empty,
                    PostalCode = u.Customers.FirstOrDefault() != null && u.Customers.FirstOrDefault().Addresses.FirstOrDefault() != null
                                    ? u.Customers.FirstOrDefault().Addresses.FirstOrDefault().PostalCode : string.Empty
                }
            ).FirstOrDefaultAsync();
        }
    }
}
