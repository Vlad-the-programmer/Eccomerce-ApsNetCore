using EcommerceRestApi.Helpers.Data.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace EcommerceRestApi.Models.Dtos
{
    public class CreateOrderCustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? FlatNumber { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nip { get; set; }

        public static CreateOrderCustomerDto ToDto(Customer? customer, UserManager<ApplicationUser> _userManager)
        {
            if (customer == null) return new();
            var user = customer.User;
            var address = customer.Addresses.FirstOrDefault();

            return new CreateOrderCustomerDto
            {
                FlatNumber = address?.FlatNumber ?? string.Empty,
                HouseNumber = address?.HouseNumber ?? string.Empty,
                City = address?.City ?? string.Empty,
                PostalCode = address?.PostalCode ?? string.Empty,
                State = address?.State ?? string.Empty,
                Street = address?.Street ?? string.Empty,
                CountryName = address?.Country?.CountryName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                FirstName = user?.FirstName ?? string.Empty,
                LastName = user?.LastName ?? string.Empty,
                PhoneNumber = user?.PhoneNumber ?? string.Empty,
                Nip = customer?.Nip ?? string.Empty,
            };
        }
    }
}
