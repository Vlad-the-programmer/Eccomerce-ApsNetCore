using EcommerceWebApp.Helpers.Enums;

namespace EcommerceWebApp.Helpers.Orders
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
    }
}
