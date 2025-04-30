namespace EcommerceWebApp.ApiServices
{
    public interface IApiService
    {
        Task<string> GetDataAsync(string endpoint);
        Task<string> PostDataAsync(string endpoint, string jsonContent = "");
        Task<string> UploadPhotoAsync(Stream fileStream, string fileName);
        Task<string> UpdateDataAsync(string endpoint, string jsonContent);
        Task<string> DeleteDataAsync(string endpoint);
    }

}
