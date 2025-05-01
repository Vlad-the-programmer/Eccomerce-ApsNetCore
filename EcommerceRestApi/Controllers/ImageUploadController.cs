using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public ImageUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsDir = Path.Combine(_env.WebRootPath,
                                        AppGlobals.AppConstants.IMAGE_UPLOAD_PATH);
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"{AppGlobals.AppConstants.IMAGE_UPLOAD_PATH}/{fileName}";
            return Ok(fileUrl);
        }

    }
}
