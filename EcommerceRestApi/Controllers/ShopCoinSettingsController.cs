using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    public class ShopCoinSettingsController : ControllerBase
    {
        private readonly IShopCoinSettingsService _service;

        public ShopCoinSettingsController(IShopCoinSettingsService service)
        {
            _service = service;
        }

        // GET: api/ShopCoinSettings
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _service.GetShopCoinSettingsAsync();

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel { Message = e.Message });
            }
        }

        // POST: api/ShopCoinSettings
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShopCoinSettingsDto dto)
        {
            try
            {
                await _service.CreateShopCoinSettingsAsync(dto);
                return Ok(new { message = "Settings created" });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel { Message = e.Message });
            }
        }

        // PUT: api/ShopCoinSettings
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ShopCoinSettingsDto dto)
        {
            try
            {
                await _service.UpdateShopCoinSettingsAsync(dto);
                return Ok(new { message = "Settings updated" });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel { Message = e.Message });
            }
        }
    }
}
