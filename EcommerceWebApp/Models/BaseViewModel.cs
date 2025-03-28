using System.Text.Json.Serialization;

namespace EcommerceWebApp.Models
{
    public class BaseViewModel
    {
        public BaseViewModel() { }
        public BaseViewModel(string title = "")
        {
            PageTitle = title;
        }
        [JsonIgnore]
        public string PageTitle { get; set; }
    }
}
