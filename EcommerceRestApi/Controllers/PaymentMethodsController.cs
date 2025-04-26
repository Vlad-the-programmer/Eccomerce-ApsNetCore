using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentMethodsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryMethodDto>>> GetPaymentMethods()
        {
            var paymentMethods = await _context.PaymentMethods
                .Select(pm => new PaymentMethodDto
                {
                    PaymentType = pm.PaymentType,
                    EnumValue = OrderProcessingFuncs.GetEnumValueForPaymentMethod(pm.PaymentType)
                })
                .ToListAsync();

            return Ok(paymentMethods);
        }
    }

}
