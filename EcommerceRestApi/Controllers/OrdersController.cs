using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using Microsoft.AspNetCore.Identity;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders.Include(o => o.Customer)
                                        .Include(o => o.OrderItems)
                                        .Select(o => new OrderViewModel
                                        {
                                            Code = o.Code,
                                            CustomerId = o.CustomerId,
                                            OrderDate = o.OrderDate,
                                            TotalAmount = o.TotalAmount,
                                            Status = o.Status,
                                            OrderItems = (IList<OrderItem>)o.OrderItems.Select(oi => new OrderItemViewModel
                                            {
                                                ProductId = oi.ProductId,
                                                Quantity = oi.Quantity,
                                                UnitPrice = oi.UnitPrice,
                                                OrderId = oi.OrderId,
                                            }).ToList()
                                        }).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetOrder(string code)
        {
            var order = await _context.Orders.Include(o => o.Customer)
                                             .Include(o => o.OrderItems)
                                             .FirstOrDefaultAsync(o => o.Code == code);

            if (order == null)
                return NotFound();

            if(order.Customer.User.Id != _userManager.GetUserId(User))
            {
                return Forbid();
            }
            var orderVM =  new OrderViewModel
            {
                Code = order.Code,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderItems = (IList<OrderItem>)order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    OrderId = oi.OrderId,
                }).ToList()
            };

            return Ok(orderVM);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder(OrderViewModel model)
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

            var order = new Order
            {
                Code = model.Code,
                CustomerId = model.CustomerId,
                OrderDate = model.OrderDate,
                TotalAmount = model.TotalAmount,
                Status = model.Status,
                OrderItems = model.OrderItems.Select(oi => new OrderItem
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    OrderId = oi.OrderId,
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { code = order.Code }, model);
        }

        [HttpPut("{code}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder(string code, OrderViewModel model)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Code == code);
            if (order == null)
                return NotFound();

            order.Status = model.Status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{code}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(string code)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Code == code);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

