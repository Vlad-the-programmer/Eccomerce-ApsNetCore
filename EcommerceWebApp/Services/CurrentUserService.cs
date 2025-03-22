using System.Data;
using System.Security.Claims;
using EcommerceWebApp.Models.AppViewModels;
using Microsoft.AspNetCore.Http;

namespace EcommerceWebApp.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CurrentUserViewModel GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (user?.Identity?.IsAuthenticated == true)
            {
                return new CurrentUserViewModel
                {
                    UserName = user.Identity.Name,
                    Email = user.FindFirst(ClaimTypes.Email)?.Value,
                    Role = role,
                    FullName = user.FindFirst(ClaimTypes.Name)?.Value,
                    IsAdmin = role.ToLower() == "admin" ? true : false
                };
            }

            return null; // Return null if the user is not authenticated
        }
    }
}
