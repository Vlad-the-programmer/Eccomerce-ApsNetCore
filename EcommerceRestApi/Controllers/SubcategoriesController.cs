using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var subCategory = await _service.GetSubcategoryByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return Ok(subCategory);
        }

        // GET: api/subCategories/code
        [HttpGet("{code:alpha}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var subCategory = await _service.GetSubcategoryByCodeAsync(code);
            if (subCategory == null)
            {
                return NotFound();
            }
            return Ok(subCategory);
        }

        [HttpPost]
        [Authorize(Policy = Permissions.ManageCategories)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NewCategoryVM))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NewSubcategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var subCategories = await _service.GetAllSubcategories();
            if (subCategories.Where(c => c.Name.Equals(model.Name)).FirstOrDefault() != null)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Subcategory with such name already exists!",
                });
            }
            await _service.AddNewSubCategoryAsync(model);
            return Created();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.ManageCategories)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubcategoryUpdateVM))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] SubcategoryUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var subcategory = await _service.GetSubcategoryByIdAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }


            await _service.UpdateSubCategoryAsync(id, model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.ManageCategories)]
        public async Task<IActionResult> Delete(int id)
        {
            var subCategory = await _service.GetSubcategoryByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
