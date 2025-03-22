using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
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

        public List<Country> GetCountriesList()
        {
            return _context.Countries.ToList();
        }
    }
}
