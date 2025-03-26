using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EcommerceWebApp.Helpers.JWT
{
    public class JWTAuth
    {
        public static void SetUserFromToken(string token, IHttpContextAccessor _httpContextAccessor)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);

            _httpContextAccessor.HttpContext.User = principal;
        }
    }
}
