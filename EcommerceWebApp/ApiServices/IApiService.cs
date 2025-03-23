using System.Text;

namespace EcommerceWebApp.ApiServices
{
    public interface IApiService
    {
        Task<string> GetDataAsync(string endpoint);
        Task<string> PostDataAsync(string endpoint, string jsonContent = "");
    }

}
