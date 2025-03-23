using System.Text.Json.Serialization;

namespace EcommerceWebApp.Models.AppViewModels
{
    public class ErrorViewModel
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
