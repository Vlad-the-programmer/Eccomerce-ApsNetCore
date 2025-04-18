using EcommerceRestApi.Helpers.Data.ViewModels;

namespace EcommerceRestApi.Helpers.Data.AuthVms
{
    public class CurrentUserViewModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAuthenticated { get; set; }
        public int? CustomerId { get; set; }

        public static explicit operator CurrentUserViewModel(ApplicationUser user)
        {
            return new CurrentUserViewModel
            {
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                IsAuthenticated = user.IsAuthenticated,
                CustomerId = user.Customers?.FirstOrDefault()?.Id
            };
        }
    }
}
