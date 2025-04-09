using EcommerceRestApi.Models;
using EcommerceRestApi.Services;
using EcommerceRestApi.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ISubcategoryService _service;

        public SubcategoriesController(ISubcategoryService service)
        {
            _service = service;
        }

        // GET: api/subCategories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subCategories = await _service.GetAllSubcategories();
            return Ok(subCategories);
        }

        // GET: api/subCategories/5
        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var subCategory = await _service.GetSubcategoryByCodeAsync(code);
            if (subCategory == null)
            {
                return NotFound();
            }
            return Ok(subCategory);
        }
    }
}
