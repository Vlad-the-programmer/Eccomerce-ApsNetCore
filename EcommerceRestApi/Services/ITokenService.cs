using EcommerceRestApi.Helpers.Data.ViewModels;

namespace EcommerceRestApi.Services
{
    public interface ITokenService
    {
        public string GenerateJwtToken(IConfiguration _configuration, ApplicationUser user);
    }
}
