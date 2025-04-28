using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;

namespace EcommerceRestApi.Helpers.Data.Functions
{
    public class OrderProcessingFuncs
    {
        public static string GetStringValue(OrderStatuses status)
        {
            return status switch
            {
                OrderStatuses.Pending => "Pending",
                OrderStatuses.Approved => "Approved",
                OrderStatuses.Rejected => "Rejected",
                OrderStatuses.Paid => "Paid",
                OrderStatuses.Cancelled => "Cancelled",

                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }

        public static string GetStringValue(PaymentMethods paymentMethod)
        {
            return paymentMethod switch
            {
                PaymentMethods.Card => "Card",
                PaymentMethods.BankTransaction => "BankTransaction",
                PaymentMethods.PayPal => "PayPal",
                PaymentMethods.Cash => "Cash",

                _ => throw new ArgumentOutOfRangeException(nameof(paymentMethod), paymentMethod, null)
            };
        }

        public static string GetStringValue(DeliveryMethods deliveryMethod)
        {
            return deliveryMethod switch
            {
                DeliveryMethods.StandardDelivery => "StandardDelivery",
                DeliveryMethods.ParcelLocker => "ParcelLocker",
                DeliveryMethods.Courier => "Courier",
                DeliveryMethods.TakeAway => "TakeAway",

                _ => throw new ArgumentOutOfRangeException(nameof(deliveryMethod), deliveryMethod, null)
            };
        }

        public static int GetEnumValueForOrderStatus(string? orderStatus)
        {
            return (int)GetOrderStatusObj(orderStatus);
        }

        public static int GetEnumValueForDeliveryMethod(string? deliveryMethod)
        {
            return (int)GetDeliveryMethodObj(deliveryMethod);
        }

        public static int GetEnumValueForPaymentMethod(string? paymentMethod)
        {
            return (int)GetPaymentMethodObj(paymentMethod);
        }

        public static OrderStatuses GetOrderStatusObj(string? orderStatus)
        {
            if (Enum.TryParse(orderStatus, ignoreCase: true, out OrderStatuses statusObj))
            {
                return statusObj;
            }
            throw new ArgumentException($"Invalid OrderStatus: {orderStatus}");

        }

        public static DeliveryMethods GetDeliveryMethodObj(string? deliveryMethod)
        {
            if (Enum.TryParse<DeliveryMethods>(deliveryMethod, ignoreCase: true, out var methodObj))
            {
                return methodObj;
            }

            throw new ArgumentException($"Invalid DeliveryMethod: {deliveryMethod}");
        }

        public static PaymentMethods GetPaymentMethodObj(string? paymentMethod)
        {
            if (Enum.TryParse<PaymentMethods>(paymentMethod, ignoreCase: true, out var methodObj))
            {
                return methodObj;
            }

            throw new ArgumentException($"Invalid PaymentMethod: {paymentMethod}");
        }

        public static async Task CreateShippment(int orderId,
                                                    DateTime estimatedArrivalDateShippment,
                                                    int deliveryMethodId,
                                                    AppDbContext context)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null) return;

            var shippment = new Shipment
            {
                DeliveryMethodId = deliveryMethodId,
                ShipmentDate = order.OrderDate.AddDays(2),
                EstimatedArrivalDate = estimatedArrivalDateShippment,
                OrderId = order.Id,
                IsActive = true,
                DateCreated = DateTime.Now
            };

            var deliveryMethod = context.DeliveryMethods.FirstOrDefault(dm =>
                                                            dm.Id == deliveryMethodId);
            if (deliveryMethod != null)
            {
                order.TotalAmount += deliveryMethod.Cost; // Add delivery cost to Total order cost 
            }

            order.Shipments.Add(shippment);
            order.DateUpdated = DateTime.Now;

            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
    }
}
