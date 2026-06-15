using EcommerceWebApp.Models.AppViewModels;

namespace EcommerceWebApp.Models.ResponseModels
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CurrentUserDTO? User { get; set; }
        public IList<string> Permissions { get; set; } = new List<string>();
    }
}
