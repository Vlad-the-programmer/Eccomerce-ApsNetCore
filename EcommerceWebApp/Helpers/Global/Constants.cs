using System.Text.Json;

public static class GlobalConstants
{
    // JSON Serializer Options
    public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };


    // API Endpoints
    public const string CountriesEndpoint = "api/countries/";

    // Products
    public const string ProductsEndpoint = "api/products";
    public const string ProductDeleteEndpoint = "api/products/delete";
    public const string ProductUpdateEndpoint = "api/products/update";
    public const string ProductCreateEndpoint = "api/products/create";
    public const string FilterProductsEndpoint = $"{ProductsEndpoint}/filter";
    public const string GetSearchComboboxDtosEndpoint = $"{ProductsEndpoint}/search-combo-box-dtos";
    public const string GetOrderByComboboxDtosEndpoint = $"{ProductsEndpoint}/order-by-combo-box-dtos";

    // Orders
    public const string OrdersEndpoint = "api/orders";
    public const string UserOrdersEndpoint = $"{OrdersEndpoint}/customer";
    public const string OrderDeleteEndpoint = "api/orders/delete";
    public const string OrderUpdateEndpoint = "api/orders/update";
    public const string OrderCreateEndpoint = "api/orders/create";
    public const string GetOrderCreateModelEndpoint = "api/orders/get-order-create-model";

    public const string FilterOrdersEndpoint = $"{OrdersEndpoint}/filter";
    public const string GetSearchComboboxDtosOrdersEndpoint = $"{OrdersEndpoint}/search-combo-box-dtos";
    public const string GetOrderByComboboxDtosOrdersEndpoint = $"{OrdersEndpoint}/order-by-combo-box-dtos";

    // Categories
    public const string CategoriesEndpoint = "api/categories";
    public const string CategoriesAdminEndpoint = "api/categories/admin";

    public const string CategoryDeleteEndpoint = "api/account";
    public const string CategoryUpdateEndpoint = "api/account";
    public const string SubCategoriesEndpoint = "api/subcategories";

    // Users
    public const string UsersEndpoint = "api/account/users/";
    public const string LoginEndpoint = "api/account/login/";
    public const string RegisterEndpoint = "api/account/register/";
    public const string LogoutEndpoint = "api/account/logout/";
    public const string AccessDeniedEndpoint = "api/account/access-denied/";
    public const string UserDeleteEndpoint = "api/account";
    public const string UserUpdateEndpoint = "api/account";
    public const string UserCreateEndpoint = "api/account/";
    public const string GetCurrentUserEndpoint = "api/account/get-current-user/";
    public const string GetUserUpdateModelEndpoint = "api/account/get-update-user-model";
    public const string GetUserProfile = "api/account/user-profile";

    // ShopppingCart
    public const string GetCartEndpoint = "api/cart/";
    public const string GetCartItemsEndpoint = "api/cart/items/";
    public const string ClearCartEndpoint = "api/cart/"; //HTTP_Delete
    public const string AddItemToCartEndpoint = "api/cart"; //HTTP_POST
    public const string RemoveItemFromCartEndpoint = "api/cart/remove-item"; // + id
    public const string GetCartProductById = "api/cart/cart-item"; // + {product_id}/

    // Refunds
    public const string RefundsEndpoint = "api/refunds";

    public const string GetSearchComboboxDtosRefundsEndpoint = $"{RefundsEndpoint}/search-combo-box-dtos";
    public const string GetOrderByComboboxDtosRefundsEndpoint = $"{RefundsEndpoint}/order-by-combo-box-dtos";

    // Returns
    public const string ReturnsEndpoint = "api/return";
    public const string GetSearchComboboxDtosReturnsEndpoint = $"{ReturnsEndpoint}/search-combo-box-dtos";
    public const string GetOrderByComboboxDtosReturnsEndpoint = $"{ReturnsEndpoint}/order-by-combo-box-dtos";

    // Reviews 
    public const string ReviewsEndpoint = "api/reviews";
    public const string ReviewCreateEndpoint = "api/reviews";
    public const string ReviewUpdateEndpoint = "api/reviews";
    public const string ReviewsDeleteEndpoint = "api/reviews";

    // DeliveryMethods
    public const string DeliveryMethodsEndpoint = "api/DeliveryMethods";

    // PaymentMethods
    public const string PaymentMethodsEndpoint = "api/PaymentMethods";

    // Invoices
    public const string InvoiceEndpoint = "api/invoice";

    // Analitics
    public const string AnaliticsEndpoint = "api/analitics";


    // Coupons
    public const string CouponEndpoint = "api/coupon";
    public const string ApplyCouponEndpoint = "api/coupon/apply";
    public const string RemoveCouponEndpoint = "api/coupon/remove-coupon";

    // Upload Photo
    public const string UploadPhotoEndpoint = "api/ImageUpload/upload-photo";


    // Other Constants
    public const int MaxRetryAttempts = 3;
    public const string DefaultCulture = "en-US";
}