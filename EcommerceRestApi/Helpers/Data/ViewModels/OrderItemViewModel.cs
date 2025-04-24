using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class OrderItemViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required.")]
        [Column(TypeName = "money")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        //public virtual Order Order { get; set; } = null!;

        //public virtual Product Product { get; set; } = null!;
        public string ProductName { get; set; }
        public string ProductBrand { get; set; }

        public static async Task<OrderItemViewModel> ToOrderItemVM(ShoppingCartItemVM cartItemVM, AppDbContext context)
        {
            var product = await context.Products.FindAsync(cartItemVM.ProductId);
            var lastOrder = await context.Orders
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefaultAsync();

            var orderId = (lastOrder?.Id ?? 0) + 1;

            if (product == null)
            {
                return new OrderItemViewModel();
            }

            return new OrderItemViewModel
            {
                OrderId = orderId,
                ProductBrand = product.Brand,
                ProductName = product.Name,
                UnitPrice = cartItemVM.ProductPrice,
                Quantity = cartItemVM.Amount,
                ProductId = cartItemVM.ProductId
            };
        }
    }
}
