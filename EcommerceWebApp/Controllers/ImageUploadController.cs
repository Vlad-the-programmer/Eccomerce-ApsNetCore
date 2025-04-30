using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers.ImageUploadHandler;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Controllers
{
    [Route("image-upload")]
    public class ImageUploadController : Controller
    {
        private readonly IApiService _apiService;

        public ImageUploadController(IApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<string> UploadImageAsync(string localFilePath)
        {
            var imageUrl = await ImageUploadHandler.UploadImageAsync(
                            localFilePath, _apiService);
            return imageUrl;

        }

    }
}
