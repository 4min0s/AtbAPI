using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly DigiBankContext _context;

        public UploadsController(DigiBankContext context)
        {
            _context = context;
        }

        // POST: api/Uploads/image?reference=DMD-20260422-ABC123&type=cin_front&context=demandeCredit
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(
            IFormFile file,
            [FromQuery] string reference,
            [FromQuery] string type,
            [FromQuery] string context = "demandeCompte") // default keeps backward compat
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file received");

            // Validate extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Only jpg, png and pdf files are allowed");

            // Build folder path based on context
            // Documents/DemandesCompte/REF123/   or
            // Documents/DemandesCredit/DMD-20260422-ABC123/
            var contextFolder = context == "demandeCredit" ? "DemandesCredit" : "DemandesCompte";
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Documents", contextFolder, reference);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // e.g. cin_front.pdf, fiche_de_paie.jpg
            var fileName = $"{type}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            // Update the correct table based on context
            if (context == "demandeCredit")
                await UpdateDemandeCreditPath(reference, type, filePath);
            else
                await UpdateDemandeComptePath(reference, type, filePath);

            return Ok(new { path = filePath });
        }

        // ── DemandeCredit path updater ────────────────────────────────────
        private async Task UpdateDemandeCreditPath(string reference, string type, string filePath)
        {
            var demande = await _context.DemandeCredits
                .Include(d => d.DemandeCreditVehicule)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierAcquisition)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierConstruction)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierRenovation)
                .FirstOrDefaultAsync(d => d.Reference == reference);

            if (demande == null) return;

            // ── Common documents (all credit types) ───────────────────────
            switch (type)
            {
                case "cin_front": demande.CinPathFront = filePath; break;
                case "cin_back": demande.CinPathBack = filePath; break;
                case "residence": demande.IndicateurResidencePath = filePath; break;
                case "fiche_de_paie": demande.FicheDePaiePath = filePath; break;
                case "attestation_travail": demande.AttestationDeTravailPath = filePath; break;
                case "attestation_salaire": demande.AttestationDeSalairePath = filePath; break;

                // ── Vehicule documents ─────────────────────────────────────
                case "facture_proforma":
                    if (demande.DemandeCreditVehicule != null)
                        demande.DemandeCreditVehicule.FactureProformaPath = filePath;
                    break;
                case "carte_grise":
                    if (demande.DemandeCreditVehicule != null)
                        demande.DemandeCreditVehicule.CarteGrisePath = filePath;
                    break;
                case "promesse_vente_vehicule":
                    if (demande.DemandeCreditVehicule != null)
                        demande.DemandeCreditVehicule.PromesseVentePath = filePath;
                    break;

                // ── Immobilier documents ───────────────────────────────────
                case "promesse_vente_immobilier":
                    if (demande.DemandeCreditImmobilier?.DemandeCreditImmobilierAcquisition != null)
                        demande.DemandeCreditImmobilier.DemandeCreditImmobilierAcquisition.PromesseVentePath = filePath;
                    break;
                case "devis_estimatif_construction":
                    if (demande.DemandeCreditImmobilier?.DemandeCreditImmobilierConstruction != null)
                        demande.DemandeCreditImmobilier.DemandeCreditImmobilierConstruction.DevisEstimatifPath = filePath;
                    break;
                case "autorisation_batir":
                    if (demande.DemandeCreditImmobilier?.DemandeCreditImmobilierConstruction != null)
                        demande.DemandeCreditImmobilier.DemandeCreditImmobilierConstruction.AutorisationBatirPath = filePath;
                    break;
                case "plan_architecte":
                    if (demande.DemandeCreditImmobilier?.DemandeCreditImmobilierConstruction != null)
                        demande.DemandeCreditImmobilier.DemandeCreditImmobilierConstruction.PlanArchitectePath = filePath;
                    break;
                case "devis_estimatif_renovation":
                    if (demande.DemandeCreditImmobilier?.DemandeCreditImmobilierRenovation != null)
                        demande.DemandeCreditImmobilier.DemandeCreditImmobilierRenovation.DevisEstimatifPath = filePath;
                    break;
            }

            await _context.SaveChangesAsync();
        }

        // ── DemandeCompte path updater (your existing logic, unchanged) ───
        private async Task UpdateDemandeComptePath(string reference, string type, string filePath)
        {
            var demande = await _context.DemandeComptes
                .FirstOrDefaultAsync(d => d.Reference == reference);

            if (demande == null) return;

            switch (type)
            {
                case "cin_front": demande.CinPathFront = filePath; break;
                case "cin_back": demande.CinPathBack = filePath; break;
                case "residence": demande.IndicateurResidencePath = filePath; break;
            }

            await _context.SaveChangesAsync();
        }





        // GET: api/Uploads/tab-amortissement/{reference}
        [HttpGet("tab-amortissement/{reference}")]
        public async Task<IActionResult> GetTabAmortissement(string reference)
        {
            var demande = await _context.Credits
                .FirstOrDefaultAsync(c => c.Reference == reference);

            if (demande == null)
                return NotFound("Credit not found");

            if (string.IsNullOrEmpty(demande.TabAmortissementPath))
                return NotFound("No amortisation table found for this credit");

            if (!System.IO.File.Exists(demande.TabAmortissementPath))
                return NotFound("File not found on server");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(demande.TabAmortissementPath);
            var fileName = $"tableau_amortissement_{reference}.pdf";

            return File(fileBytes, "application/pdf", fileName);
        }
    }
}