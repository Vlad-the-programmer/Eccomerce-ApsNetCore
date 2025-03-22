using EcommerceRestApi.Models;
using EcommerceRestApi.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public interface ICountryService: IEntityBaseRepository<Country>
    {
        public List<Country> GetCountriesList();
    }
}
