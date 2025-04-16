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

        public async static Task<Invoice> GenerateInvoice(Order item, AppDbContext context)
        {
            Invoice invoice = new Invoice()
            {
                IsActive = true,
                IsPaid = false,
                DateOfIssue = DateTime.Now,
                DateCreated = DateTime.Now,
                Number = GenerateInvoiceNumber(),
                CustomerId = item.CustomerId,
            };

            invoice.PaymentId = (await GetPaymentByOrderId(item.Id, context))?.Id;

            foreach (var orderItem in item.OrderItems)
            {
                InvoiceItem invoiceItem = new InvoiceItem()
                {
                    BasePricePerUnit = orderItem.UnitPrice,
                    DateCreated = DateTime.Now,
                    Discount = 0,
                    IsActive = true,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    TaxRate = 1.2,
                };
                invoice.InvoiceItems.Add(invoiceItem);

                if (invoice.PaymentId != null)
                {
                    await MarkInvoiceAsPaid(invoice.Id, context);
                }

                await context.InvoiceItems.AddAsync(invoiceItem);
                await context.SaveChangesAsync();
            }
            await context.Invoices.AddAsync(invoice);
            return invoice;
        }

        /// <summary>
        /// Marks an invoice as paid.
        /// </summary>
        public async static Task<bool> MarkInvoiceAsPaid(int invoiceId, AppDbContext context)
        {
            var invoice = await context.Invoices.FindAsync(invoiceId);
            if (invoice == null)
                return false;

            invoice.IsPaid = true;
            invoice.DateUpdated = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        //public Payment CreatePayment(Order model, int selectedPaymentMethodId)
        //{
        //    return new Payment()
        //    {
        //        PaymentDate = model.OrderDate,
        //        Amount = model.TotalAmount,
        //        DateCreated = DateTime.Now,
        //        OrderId = model.Id,
        //        PaymentMethodId = selectedPaymentMethodId,
        //    };
        //}

        public async static Task<Payment?> GetPaymentByOrderId(int orderId, AppDbContext context)
        {
            return await context.Payments
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }
    }
}
