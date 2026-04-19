using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // ← ajout
using TodoApi.Models;                 // ← ajout

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        // ← ajout
        private readonly DigiBankContext _context;
        public UploadsController(DigiBankContext context)
        {
            _context = context;
        }

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

            // ← ajout
            var demande = await _context.DemandeComptes
                .FirstOrDefaultAsync(d => d.Reference == reference);
            if (demande != null)
            {
                switch (type)
                {
                    case "cin_front": demande.CinPathFront = filePath; break;
                    case "cin_back": demande.CinPathBack = filePath; break;
                    case "residence": demande.IndicateurResidencePath = filePath; break;
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { path = filePath });
        }
    }
}