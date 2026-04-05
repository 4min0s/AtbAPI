using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string reference, [FromQuery] string type)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file received");

            // e.g. Documents/Demandes/ATB-EP-2026-241287/
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "Demandes", reference);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // e.g. cin_front.jpg, cin_back.jpg, residence.jpg
            var fileName = $"{type}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            return Ok(new { path = filePath });
        }
    }
}