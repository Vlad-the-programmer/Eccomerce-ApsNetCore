using EcommerceRestApi.Helpers.Enums;

namespace EcommerceRestApi.Helpers.Data.Functions
{
    public class OrderOprocessingFuncs
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
