using System.Text.Json;

public static class GlobalConstants
{
    // JSON Serializer Options
    public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // API Endpoints
    public const string CountriesEndpoint = "api/countries";
    public const string UsersEndpoint = "api/users";
    public const string ProductsEndpoint = "api/products";
    public const string CategoriesEndpoint = "api/categories";
    public const string LoginEndpoint = "api/account/login";
    public const string RegisterEndpoint = "api/account/register";
    public const string LogoutEndpoint = "api/account/logout";
    public const string AccessDeniedEndpoint = "api/account/access-denied";


    // Other Constants
    public const int MaxRetryAttempts = 3;
    public const string DefaultCulture = "en-US";
}