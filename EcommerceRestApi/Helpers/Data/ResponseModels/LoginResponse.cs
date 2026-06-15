using EcommerceRestApi.Helpers.Data.AuthVms;

namespace EcommerceRestApi.Helpers.Data.ResponseModels
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public IList<string> Permissions { get; set; } = new List<string>();

        public CurrentUserViewModel? User { get; set; }
    }
}
