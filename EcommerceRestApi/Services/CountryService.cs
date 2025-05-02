using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public class CountryService : EntityBaseRepository<Country>, ICountryService
    {
        private readonly AppDbContext _context;

        public CountryService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public List<CountryDTO> GetCountriesList()
        {
            return _context.Countries.Select(c => new CountryDTO()
            {
                Id = c.Id,
                CountryCode = c.CountryCode,
                CountryName = c.CountryName
            }).ToList();
        }
    }
}
