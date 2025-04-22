using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _service;

        public CountriesController(AppDbContext dbContext, ICountryService service)
        {
            _service = service;
        }

        // GET: api/countries
        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = _service.GetCountriesList();
            return Ok(countries);
        }

    }
}
