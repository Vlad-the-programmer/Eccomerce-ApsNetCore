using EcommerceRestApi.Models;
using Microsoft.AspNetCore.Identity;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class CustomerViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nip { get; set; } = string.Empty;
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? CountryName { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? FlatNumber { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public static CustomerViewModel ToVM(Customer? customer, UserManager<ApplicationUser> _userManager)
        {
            if (customer == null) return new();
            var user = _userManager.Users.FirstOrDefault(u => u.Id == customer.UserId);
            var address = customer.Addresses.FirstOrDefault();

            return new CustomerViewModel
            {
                FlatNumber = address?.FlatNumber,
                HouseNumber = address?.HouseNumber,
                City = address?.City,
                PostalCode = address?.PostalCode,
                State = address?.State,
                Street = address?.Street,
                CountryName = address?.Country?.CountryName,
                Email = user?.Email ?? string.Empty,
                FirstName = user?.FirstName ?? string.Empty,
                LastName = user?.LastName ?? string.Empty,
                PhoneNumber = user?.PhoneNumber ?? string.Empty,
                Nip = customer?.Nip,
                IsActive = customer.IsActive,
                IsAdmin = customer.User.IsAdmin,
                IsAuthenticated = customer.User.IsAuthenticated,
                Role = customer.User.Role,
                UserName = customer.User.UserName,
            };
        }
    }
}
