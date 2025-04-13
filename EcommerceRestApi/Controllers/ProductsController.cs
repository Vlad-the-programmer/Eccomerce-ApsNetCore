using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IProductsService _service;

        public ProductsController(AppDbContext dbContext, IProductsService service)
        {
            _dbContext = dbContext;
            _service = service;
        }


        [HttpGet("filter")]
        public async Task<IActionResult> Filter(string searchString)
        {
            try
            {
                // Fetch all products
                var allProducts = await _service.GetAllAsync(n => n.ProductCategories);

                // Filter products if searchString is provided
                if (!string.IsNullOrEmpty(searchString))
                {
                    var filteredResult = allProducts
                        .Where(n => n.Name.ToLower().Contains(searchString.ToLower()) ||
                                    n.LongAbout.ToLower().Contains(searchString.ToLower()) ||
                                    n.LongAbout.ToLower().Contains(searchString.ToLower()))
                        .ToList();

                    return Ok(filteredResult); // Return 200 OK with filtered results
                }

                return Ok(allProducts); // Return 200 OK with all products
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    Message = "An error occurred while filtering products.",
                    Errors = new List<string>().Append(ex.Message)
                });
            }
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetProductByIDAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/products
        //[Authorize(Roles = UserRoles.Admin)]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseModel))]
        public async Task<IActionResult> Create([FromBody] NewProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList()
                });
            }

            await _service.AddNewProductAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // PUT: api/products/5
        //[Authorize(Roles = UserRoles.Admin)]
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList()
                });
            }

            var product = await _service.GetProductByIDAsync(id);
            if (product == null)
            {
                return NotFound();
            }


            await _service.UpdateProductAsync(id, model);
            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _service.GetProductByIDAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
