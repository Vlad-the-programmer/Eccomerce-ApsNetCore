namespace EcommerceRestApi.AppGlobals
{
    public class AppConstants
    {
        public const string BASE_URL = "https://localhost:5001";
        public static string[] CLIENT_URLS = [
             "https://localhost:7006",  // Client HTTPS
            "http://localhost:5277",   // Client HTTP
            "http://localhost:5000",   // Server HTTP (if needed for client)
            "https://localhost:5001"   // Server HTTPS (if client calls directly)
            ];

        // Images
        public const string IMAGE_UPLOAD_PATH = "Uploads";

        // Points user get for making an order
        public const decimal POINTS_PER_DOLLAR = 1.5m;
        public const decimal TAXES_RATE = 0.23m; // POLAND
    }
}
