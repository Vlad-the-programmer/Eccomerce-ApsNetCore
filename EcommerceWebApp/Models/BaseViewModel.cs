namespace EcommerceWebApp.Models
{
    public class BaseViewModel
    {
        public BaseViewModel(string title = "")
        {
            PageTitle = title;
        }
        public string PageTitle { get; set; }
    }
}
