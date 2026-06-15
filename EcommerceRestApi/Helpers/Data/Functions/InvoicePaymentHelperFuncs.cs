using EcommerceRestApi.AppGlobals;
using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Helpers.Data.Functions
{
    public class InvoicePaymentHelperFuncs
    {

        /// <summary>
        /// Generates a unique invoice number.
        /// </summary>
        private static string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }

        /// <summary>
        /// Generates an invoice for a completed payment.
        /// </summary>

        public async static Task<Invoice?> GenerateInvoice(Order? item, AppDbContext context)
        {
            if (item == null) return null;

            Invoice invoice = new Invoice()
            {
                IsActive = true,
                IsPaid = false,
                DateOfIssue = DateTime.Now,
                DateCreated = DateTime.Now,
                Number = GenerateInvoiceNumber(),
                CustomerId = item.CustomerId,
            };


            await context.Invoices.AddAsync(invoice);
            await context.SaveChangesAsync();

            var discountForItem = (double)await GetOrderCouponsTotalDiscount(item.OrderCoupons) / item.OrderItems.Count;

            foreach (var orderItem in item.OrderItems)
            {
                InvoiceItem invoiceItem = new InvoiceItem()
                {
                    InvoiceId = invoice.Id,
                    BasePricePerUnit = orderItem.UnitPrice,
                    DateCreated = DateTime.Now,
                    Discount = discountForItem,
                    IsActive = true,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    TaxRate = (double)AppConstants.TAXES_RATE,
                };

                await context.InvoiceItems.AddAsync(invoiceItem);
            }

            await context.SaveChangesAsync();
            return await context.Invoices
               .Include(i => i.InvoiceItems)
                   .ThenInclude(it => it.Product)
               .FirstOrDefaultAsync(i => i.Id == invoice.Id);
        }


        /// <summary>
        /// Marks an invoice as paid.
        /// </summary>
        private async static Task<bool> MarkInvoiceAsPaid(int invoiceId, Order order, AppDbContext context)
        {
            var invoice = await context.Invoices.FindAsync(invoiceId);
            if (invoice == null)
                return false;

            var payment = await GetPaymentByOrderId(order.Id, context);
            if (payment == null)
            {
                invoice.IsPaid = false;
                invoice.DateUpdated = DateTime.Now;
                await context.SaveChangesAsync();
                return false;
            }

            invoice.PaymentId = payment.Id;
            invoice.IsPaid = true;
            invoice.DateUpdated = DateTime.Now;

            await context.SaveChangesAsync();
            return true;
        }

        public static async Task CreatePayment(Order order, int paymentMethodId, int invoiceId, AppDbContext context)
        {
            var payment = new Payment
            {
                Amount = order.TotalAmount,
                OrderId = order.Id,
                IsActive = true,
                PaymentDate = DateTime.Now,
                PaymentMethodId = paymentMethodId,
                DateCreated = DateTime.Now,
            };

            order.Payments.Add(payment);
            order.Status = OrderProcessingFuncs.GetStringValue(OrderStatuses.Paid);
            order.IsPaid = true;
            order.DateUpdated = DateTime.Now;

            context.Orders.Update(order);
            await context.SaveChangesAsync();

            await MarkInvoiceAsPaid(invoiceId, order, context);
        }

        public async static Task<Payment?> GetPaymentByOrderId(int orderId, AppDbContext context)
        {
            return await context.Payments
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async static Task<decimal> GetOrderCouponsTotalDiscount(ICollection<OrderCoupon> orderCoupons)
        {
            decimal discountTotal = decimal.Zero;
            foreach (var coupon in orderCoupons)
            {
                discountTotal += coupon.DiscountApplied;
            }
            return discountTotal;
        }
    }
}
