using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel(string title) : base(title) { Message = ""; }
        public LoginViewModel() : base("") { Message = ""; }


        [Display(Name = "EmailAddress")]
            [Required(ErrorMessage = "Email is Required")]
            public string EmailAddress { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            public bool RememberMe { get; set; }
    }
}
