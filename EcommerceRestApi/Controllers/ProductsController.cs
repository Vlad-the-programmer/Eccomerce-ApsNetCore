using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _service;

        public ProductsController(IProductsService service)
        {
            _service = service;
        }


        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
                [FromQuery] string searchString = "",
                [FromQuery] string? searchProperty = null,
                [FromQuery] string? sortProperty = null,
                [FromQuery] decimal? fromPrice = null,
                [FromQuery] decimal? ToPrice = null,
                [FromQuery] string? categoryCode = null,
                [FromQuery] string? subcategoryCode = null,
                [FromQuery] int? minRating = null,
                [FromQuery] bool sortAscending = false)
        {
            try
            {
                var filteredProducts = await _service.FilterProductsAsync(
                    searchString, searchProperty, sortProperty, fromPrice, ToPrice, categoryCode, subcategoryCode, minRating, sortAscending);
                return Ok(filteredProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    Message = "An error occurred while filtering products.",
                    Errors = new List<string>().Append(ex.Message).ToList()
                });
            }
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetProducts();
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
        [HttpPost("create")]
        [Authorize(Policy = Permissions.ManageProduct)]
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
        [HttpPut("update/{id}")]
        [Authorize(Policy = Permissions.ManageProduct)]
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
        [Authorize(Policy = Permissions.ManageProduct)]
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

        [HttpGet("search-combo-box-dtos")]
        public IActionResult GetSearchComboBoxDtos()
        {
            var dtos = _service.GetSearchComboBoxDtos();
            return Ok(dtos);
        }

        [HttpGet("order-by-combo-box-dtos")]
        public IActionResult GetOrderByComboBoxDtos()
        {
            var dtos = _service.GetOrderByComboBoxDtos();
            return Ok(dtos);
        }

        [HttpPost("increase-stock")]
        [Authorize(Policy = Permissions.ManageProduct)]
        public async Task<IActionResult> IncreaseStock([FromBody] IncreaseStockRequest request)
        {
            try
            {
                var result = await _service.IncreaseStockAsync(request.ProductId, request.QuantityToAdd);
                return Ok(new { newStock = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class IncreaseStockRequest
        {
            public int ProductId { get; set; }
            public int QuantityToAdd { get; set; }
        }
    }
}
