namespace EcommerceMobileApp.AppLogic.Dtos
{
    public class CurrentUserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }

        public bool IsAuthenticated { get; set; }
        public int? CustomerId { get; set; }
        public string Role { get; set; }

    }
}
