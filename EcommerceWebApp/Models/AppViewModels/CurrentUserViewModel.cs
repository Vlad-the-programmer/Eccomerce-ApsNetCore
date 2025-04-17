namespace EcommerceWebApp.Models.AppViewModels
{
    public class CurrentUserViewModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAuthenticated { get; set; } = default!;

    }
}
