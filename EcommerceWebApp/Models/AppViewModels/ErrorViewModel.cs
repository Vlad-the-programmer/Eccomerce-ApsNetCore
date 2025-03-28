using System.Text.Json.Serialization;

namespace EcommerceWebApp.Models.AppViewModels
{
    public class ErrorViewModel
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("errors")]
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
