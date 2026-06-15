using System.Security.Claims;

namespace EcommerceWebApp.Helpers.Permissions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.Claims.Any(c =>
                c.Type == "Permission" && c.Value == permission);
        }
    }
}
