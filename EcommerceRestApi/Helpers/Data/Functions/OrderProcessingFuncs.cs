using EcommerceRestApi.Helpers.Enums;

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
                PaymentMethods.Card => "Debit/Creadit card",
                PaymentMethods.Transaction => "Bank transaction",
                PaymentMethods.PayPal => "PayPal",
                PaymentMethods.Cash => "Cash",

                _ => throw new ArgumentOutOfRangeException(nameof(paymentMethod), paymentMethod, null)
            };
        }

        public static string GetStringValue(DeliveryMethods deliveryMethod)
        {
            return deliveryMethod switch
            {
                DeliveryMethods.Delivery => "Statndard delivery",
                DeliveryMethods.ParcelLocker => "Parcel Locker",
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
            Enum.TryParse(orderStatus, ignoreCase: true, out OrderStatuses statusObj);
            return statusObj;
        }

        public static DeliveryMethods GetDeliveryMethodObj(string? deliveryMethod)
        {
            Enum.TryParse(deliveryMethod, ignoreCase: true, out DeliveryMethods methodObj);
            return methodObj;
        }

        public static PaymentMethods GetPaymentMethodObj(string? paymentMethod)
        {
            Enum.TryParse(paymentMethod, ignoreCase: true, out PaymentMethods methodObj);
            return methodObj;
        }
    }
}
