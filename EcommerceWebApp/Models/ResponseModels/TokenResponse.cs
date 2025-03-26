namespace EcommerceWebApp.Models.ResponseModels
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public ApplicationUserViewModel User { get; set; }
    }
}
