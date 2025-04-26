using System.Text.Json;

public static class GlobalConstants
{
    // JSON Serializer Options
    public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
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

    // Products
    public const string OrdersEndpoint = "api/orders";
    public const string OrderDeleteEndpoint = "api/orders/delete";
    public const string OrderUpdateEndpoint = "api/orders/update";
    public const string OrderCreateEndpoint = "api/orders/create";
    public const string GetOrderCreateModelEndpoint = "api/orders/get-order-create-model";

    // Categories
    public const string CategoriesEndpoint = "api/categories/";
    public const string SubCategoriesEndpoint = "api/subcategories/";

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

    // ShopppingCart
    public const string GetCartEndpoint = "api/cart/";
    public const string GetCartItemsEndpoint = "api/cart/items/";
    public const string ClearCartEndpoint = "api/cart/"; //HTTP_Delete
    public const string AddItemToCartEndpoint = "api/cart"; //HTTP_POST
    public const string RemoveItemFromCartEndpoint = "api/cart/remove-item"; // + id
    public const string GetCartProductById = "api/cart/cart-item"; // + {product_id}/

    // Reviews 
    public const string ReviewsEndpoint = "api/reviews";
    public const string ReviewCreateEndpoint = "api/reviews";
    public const string ReviewUpdateEndpoint = "api/reviews";
    public const string ReviewsDeleteEndpoint = "api/reviews";

    // DeliveryMethods
    public const string DeliveryMethodsEndpoint = "api/DeliveryMethods";

    // PaymentMethods
    public const string PaymentMethodsEndpoint = "api/PaymentMethods";

    // Other Constants
    public const int MaxRetryAttempts = 3;
    public const string DefaultCulture = "en-US";
}