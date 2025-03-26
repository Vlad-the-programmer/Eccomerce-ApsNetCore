using EcommerceRestApi.Helpers.Data.ViewModels;

namespace EcommerceRestApi.Services
{
    public interface ITokenService
    {
        public string GenerateToken(IConfiguration _configuration, ApplicationUser user);
    }
}
