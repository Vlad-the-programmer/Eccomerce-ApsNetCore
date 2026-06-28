namespace EcommerceWebApp.Helpers.Permissions
{
    public static class Permissions
    {
        public const string ManageProduct = "permissions.manage_products";
        public const string ManageCoupons = "permissions.manage_coupons";
        public const string ManageCategories = "permissions.manage_categories";
        public const string ManageUsers = "permissions.manage_users";
        public const string ManageOrders = "permissions.manage_orders";
        public const string ManageRefunds = "permissions.manage_refunds";

        public static List<string> AllPermissions => new()
            {
                ManageCategories,
                ManageUsers,
                ManageCoupons,
                ManageOrders,
                ManageProduct,
                ManageRefunds
            };
    }
}
