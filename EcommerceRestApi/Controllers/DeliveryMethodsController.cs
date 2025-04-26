using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DeliveryMethodsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DeliveryMethodsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryMethodDto>>> GetDeliveryMethods()
    {
        var deliveryMethods = await _context.DeliveryMethods
            .Select(dm => new DeliveryMethodDto
            {
                MethodName = dm.MethodName,
                Cost = dm.Cost,
                EnumValue = OrderProcessingFuncs.GetEnumValueForDeliveryMethod(dm.MethodName)
            })
            .ToListAsync();

        return Ok(deliveryMethods);
    }
}
