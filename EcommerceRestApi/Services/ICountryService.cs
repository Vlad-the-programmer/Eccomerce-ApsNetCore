using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public interface ICountryService: IEntityBaseRepository<Country>
    {
        public List<CountryDTO> GetCountriesList();
    }
}
