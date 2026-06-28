using EcommerceRestApi.AppGlobals;
using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EcommerceRestApi.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        public InvoiceService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
            _notificationService = notificationService;
        }

        public async Task<InvoiceDto> GetInvoiceByOrderCodeAsync(string orderCode)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Addresses)
                        .ThenInclude(a => a.Country)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                    .ThenInclude(p => p.PaymentMethod)
                .Include(o => o.OrderCoupons)
                .FirstOrDefaultAsync(o => o.Code == orderCode);

            if (order == null)
                throw new Exception($"Order with code {orderCode} not found");

            var invoiceobj = await _context.Invoices
                                            .Include(i => i.Order)
                                                .ThenInclude(o => o.OrderCoupons)
                                            .Include(i => i.InvoiceItems)
                                                .ThenInclude(it => it.Product)
                                            .FirstOrDefaultAsync(i => i.OrderId == order.Id);
            if (invoiceobj == null)
            {
                invoiceobj = await InvoicePaymentHelperFuncs.GenerateInvoice(order, _context, _notificationService);

                if (invoiceobj == null)
                {
                    throw new Exception("Failed to generate invoice for order");
                }
            }

            var invoice = new InvoiceDto
            {
                Id = order.Id,
                Number = invoiceobj.Number,
                DateOfIssue = invoiceobj.DateOfIssue,
                PaymentDate = invoiceobj?.Payment != null ? invoiceobj.Payment.PaymentDate : invoiceobj.DateOfIssue.AddDays(14),
                PaymentMethod = order.Payments?.FirstOrDefault()?.PaymentMethod?.PaymentType ?? "Bank Transfer",
                IsPaid = order.IsPaid,
                OrderCode = order.Code,
                Customer = new CustomerInfoDto
                {
                    FullName = order.Customer.User?.FullName ?? $"{order.Customer.User?.FirstName} {order.Customer.User?.LastName}",
                    Email = order.Customer.User?.Email ?? string.Empty,
                    PhoneNumber = order.Customer.User?.PhoneNumber ?? string.Empty,
                    Address = order.Customer.Addresses?.FirstOrDefault() != null
                        ? $"{order.Customer.Addresses.First().Street} {order.Customer.Addresses.First().HouseNumber}, {order.Customer.Addresses.First().City}, {order.Customer.Addresses.First().PostalCode}, {order.Customer.Addresses.First().Country?.CountryName}"
                        : "No address provided",
                    Nip = order.Customer.Nip
                }
            };

            decimal subtotal = 0;
            decimal taxAmount = 0;
            var discountTotal = await InvoicePaymentHelperFuncs.GetOrderCouponsTotalDiscount(order.OrderCoupons);
            var discountForItem = (double)(discountTotal / (decimal)invoiceobj.InvoiceItems.Count());

            foreach (var item in invoiceobj.InvoiceItems)
            {
                var totalPrice = item.Product.Price * (decimal)item.Quantity;
                var itemTax = totalPrice * (decimal)(AppConstants.TAXES_RATE / 100);

                subtotal += totalPrice;
                taxAmount += itemTax;

                invoice.InvoiceItems.Add(new InvoiceItemDto
                {
                    ProductName = item.Product.Name,
                    ProductBrand = item.Product.Brand,
                    BasePricePerUnit = item.Product.Price,
                    Quantity = item.Quantity,
                    TaxRate = (double)AppConstants.TAXES_RATE,
                    Discount = discountForItem,
                    TotalPrice = totalPrice,
                    ProductId = item.ProductId,
                    InvoiceId = item.InvoiceId
                });
            }


            invoice.Subtotal = subtotal;
            invoice.TaxAmount = taxAmount;
            invoice.DiscountTotal = discountTotal;
            invoice.TotalAmount = (subtotal + taxAmount) - discountTotal;

            return invoice;
        }

        public async Task<byte[]> GeneratePdfInvoiceAsync(string orderCode)
        {
            var invoice = await GetInvoiceByOrderCodeAsync(orderCode);

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin((float)1.5, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Content().Column(column =>
                    {
                        // Header with Company Info and Invoice Details
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("YOUR COMPANY NAME").Bold().FontSize(14);
                                col.Item().Text("123 Business Street");
                                col.Item().Text("City, State 12345");
                                col.Item().Text("Phone: (555) 123-4567");
                                col.Item().Text("Email: info@company.com");
                            });

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight().Text("INVOICE").Bold().FontSize(18).FontColor(Colors.Blue.Medium);
                                col.Item().AlignRight().Text($"Invoice Number: {invoice.Number}");
                                col.Item().AlignRight().Text($"Date of Issue: {invoice.DateOfIssue:MMMM dd, yyyy}");
                                col.Item().AlignRight().Text($"Payment Due: {invoice.PaymentDate:MMMM dd, yyyy}");
                                col.Item().AlignRight().Text($"Order Code: {invoice.OrderCode}");
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal((float)0.5);

                        // Bill To Section
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Bill To:").Bold().FontSize(11);
                                col.Item().PaddingLeft(10).Column(bill =>
                                {
                                    bill.Item().Text(invoice.Customer.FullName);
                                    bill.Item().Text(invoice.Customer.Email);
                                    bill.Item().Text(invoice.Customer.PhoneNumber);
                                    bill.Item().Text(invoice.Customer.Address);
                                    if (!string.IsNullOrEmpty(invoice.Customer.Nip))
                                        bill.Item().Text($"NIP: {invoice.Customer.Nip}");
                                });
                            });

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight().Text($"Status:").Bold();
                                col.Item().AlignRight().Text(invoice.IsPaid ? "PAID" : "PENDING")
                                    .FontColor(invoice.IsPaid ? Colors.Green.Medium : Colors.Orange.Medium)
                                    .Bold();
                            });
                        });

                        column.Item().PaddingTop(15).LineHorizontal((float)0.5);

                        // Items Table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);  // Product
                                columns.RelativeColumn(2);  // Brand
                                columns.ConstantColumn(50);  // Qty
                                columns.ConstantColumn(70);  // Unit Price
                                columns.ConstantColumn(50);  // Tax
                                columns.ConstantColumn(70);  // Discount
                                columns.ConstantColumn(80);  // Total
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Product").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Brand").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Qty").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Unit Price").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Tax").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Discount").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Total").Bold();
                            });

                            // Table Rows
                            foreach (var item in invoice.InvoiceItems)
                            {
                                table.Cell().Padding(5).Text(item.ProductName);
                                table.Cell().Padding(5).Text(item.ProductBrand);
                                table.Cell().Padding(5).AlignRight().Text(item.Quantity.ToString("F0"));
                                table.Cell().Padding(5).AlignRight().Text($"${item.BasePricePerUnit:F2}");
                                table.Cell().Padding(5).AlignRight().Text($"{item.TaxRate:F0}%");
                                table.Cell().Padding(5).AlignRight().Text($"${item.Discount:F2}");
                                table.Cell().Padding(5).AlignRight().Text($"${item.TotalPrice:F2}");
                            }
                        });

                        // Totals Section
                        column.Item().PaddingTop(15).AlignRight().Column(col =>
                        {
                            col.Item().Padding(2).Row(row =>
                            {
                                row.RelativeColumn(2).Text("Subtotal:");
                                row.RelativeColumn(1).Text($"${invoice.Subtotal:F2}");
                            });
                            col.Item().Padding(2).Row(row =>
                            {
                                row.RelativeColumn(2).Text("Tax:");
                                row.RelativeColumn(1).Text($"${invoice.TaxAmount:F2}");
                            });
                            if (invoice.DiscountTotal > 0)
                            {
                                col.Item().Padding(2).Row(row =>
                                {
                                    row.RelativeColumn(2).Text("Discount:");
                                    row.RelativeColumn(1).Text($"-${invoice.DiscountTotal:F2}");
                                });
                            }
                            col.Item().Padding(2).Row(row =>
                            {
                                row.RelativeColumn(2).Text("Total:").Bold();
                                row.RelativeColumn(1).Text($"${invoice.TotalAmount:F2}").Bold();
                            });
                        });

                        column.Item().PaddingTop(20).LineHorizontal((float)0.5);

                        // Payment Information
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Payment Information:").Bold().FontSize(11);
                                col.Item().PaddingLeft(10).Column(payment =>
                                {
                                    payment.Item().Text($"Payment Method: {invoice.PaymentMethod}");
                                    payment.Item().Text("Bank: Example Bank");
                                    payment.Item().Text("Account Name: Your Company Name");
                                    payment.Item().Text("Account Number: 1234 5678 9012 3456");
                                });
                            });
                        });

                        // Footer Note
                        column.Item().PaddingTop(20).AlignCenter().Text(
                            "Thank you for your business! Please include the invoice number with your payment.")
                            .FontSize(8).FontColor(Colors.Grey.Medium);
                    });

                    page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            return document.GeneratePdf();
        }

    }
}
