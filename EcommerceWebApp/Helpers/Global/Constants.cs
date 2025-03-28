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
    public const string ProductsEndpoint = "api/products/";
    public const string ProductDeleteEndpoint = "api/account/";
    public const string ProductUpdateEndpoint = "api/account/";
    public const string ProductCreateEndpoint = "api/account/";

    // Categories
    public const string CategoriesEndpoint = "api/categories/";
    public const string SubCategoriesEndpoint = "api/subcategories/";

    // Users
    public const string UsersEndpoint = "api/users/";
    public const string LoginEndpoint = "api/account/login/";
    public const string RegisterEndpoint = "api/account/register/";
    public const string LogoutEndpoint = "api/account/logout/";
    public const string AccessDeniedEndpoint = "api/account/access-denied/";
    public const string UserDeleteEndpoint = "api/account/";
    public const string UserUpdateEndpoint = "api/account/";
    public const string UserCreateEndpoint = "api/account/";
    public const string GetCurrentUserEndpoint = "api/account/get-current-user";


    // Other Constants
    public const int MaxRetryAttempts = 3;
    public const string DefaultCulture = "en-US";
}