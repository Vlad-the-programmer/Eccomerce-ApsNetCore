using EcommerceWebApp.ApiServices;

namespace EcommerceWebApp.Helpers.ImageUploadHandler
{
    public class ImageUploadHandler
    {
        public static async Task<string> UploadImageAsync(string localFilePath, IApiService apiService)
        {
            var fileName = Path.GetFileName(localFilePath);

            using var fileStream = System.IO.File.OpenRead(localFilePath);


            var imgUrl = "";
            try
            {
                imgUrl = await apiService.UploadPhotoAsync(fileStream, fileName);
            }
            catch (HttpRequestException ex)
            {
                imgUrl = "";
            }

            return imgUrl;
        }
    }
}
