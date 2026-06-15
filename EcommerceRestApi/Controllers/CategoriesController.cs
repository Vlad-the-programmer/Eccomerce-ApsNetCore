using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(AppDbContext dbContext, ICategoryService service)
        {
            _service = service;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _service.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet("admin")]
        [Authorize(Policy = Permissions.ManageCategories)]
        public async Task<IActionResult> GetAllCategoriesForAdmin()
        {
            var categories = await _service.GetAllCategoriesForAdmin();
            return Ok(categories);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetCategoryByIDAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        [Authorize(Policy = Permissions.ManageCategories)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NewCategoryVM))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NewCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categories = await _service.GetAllCategories();
            if (categories.Where(c => c.Name.Equals(model.Name)).FirstOrDefault() != null)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Category with such name already exists!",
                });
            }
            await _service.AddNewCategoryAsync(model);
            return Created();
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.ManageCategories)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryUpdateVM))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                }).ToList();

                return BadRequest(new ResponseModel
                {
                    Message = "Validation failed",
                    Errors = (IList<string>)errors
                });
            }

            var category = await _service.GetCategoryByIDAsync(id);
            if (category == null)
            {
                return NotFound();
            }


            await _service.UpdateCategoryAsync(id, model);
            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.ManageCategories)]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _service.GetCategoryByIDAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
