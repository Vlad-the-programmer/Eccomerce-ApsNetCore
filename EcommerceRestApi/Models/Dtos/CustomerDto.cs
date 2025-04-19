using EcommerceRestApi.Helpers.ModelsUtils;

namespace EcommerceRestApi.Models.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? CountryName { get; set; } = null;
        public string FullName { get; set; } = null!;
        public AddressDto? Address { get; set; } = null;
        public string? Email { get; set; } = null!;
        public string? Nip { get; set; } = null!;
        public int? Points { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }

        public static CustomerDto ToDto(Customer customer)
        {
            if (customer == null) return new();

            var dto = new CustomerDto();
            dto.CopyProperties(customer);

            dto.CountryName = customer.Addresses.FirstOrDefault()?.Country?.CountryName;
            dto.FullName = customer.User.FullName;
            dto.Address = AddressDto.ToDto(customer.Addresses.FirstOrDefault());
            dto.Email = customer.User?.Email;

            return dto;

            //return new CustomerDto
            //{
            //    Id = customer.Id,
            //    CountryName = customer.Addresses.FirstOrDefault()?.Country?.CountryName,
            //    FullName = customer.User.FullName,
            //    Address = customer.Addresses.FirstOrDefault(),
            //    Email = customer.User?.Email,
            //    Nip = customer.Nip,
            //    Points = customer.Points,
            //    IsActive = customer.IsActive,
            //    DateCreated = customer.DateCreated
            //};
        }
    }
}
