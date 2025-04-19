using EcommerceRestApi.Helpers.ModelsUtils;

namespace EcommerceRestApi.Models.Dtos
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string? CountryName { get; set; }
        public int CustomerId { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? FlatNumber { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public static AddressDto ToDto(Address? address)
        {
            if (address == null) return new();
            var dto = new AddressDto();
            dto.CopyProperties(address);

            dto.CountryName = address?.Country?.CountryName ?? string.Empty;
            return dto;
        }
    }
}
