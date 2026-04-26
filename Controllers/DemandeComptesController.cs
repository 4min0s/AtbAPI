using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemandeComptesController : ControllerBase
    {
        private readonly DigiBankContext _context;
        private readonly PdfGenerationService _pdfService;
        private readonly EmailService _emailService;

        public DemandeComptesController(
            DigiBankContext context,
            PdfGenerationService pdfService,
            EmailService emailService)
        {
            _context = context;
            _pdfService = pdfService;
            _emailService = emailService;
        }

        // GET: api/DemandeComptes
        [HttpGet]
        public async Task<IActionResult> GetDemandeComptes()
        {
            try
            {
                var demandes = await _context.DemandeComptes
                    .Select(d => new
                    {
                        d.Id,
                        d.IdClient,
                        d.IdAgence,
                        d.DateEnvoi,
                        d.TypeCompte,
                        d.Etat,
                        d.DemandePdfPath,
                        d.Reference,
                        d.Statut,
                        d.Adresse,
                        d.Gouvernorat,
                        d.Civilite,
                        d.Ville,
                        d.CodePostal,
                        d.Profession,
                        d.NomEmployeur,
                        d.Devise,
                        d.RevenuMensuel,
                        d.Nom,
                        d.Prenom,
                        d.Cin,
                        d.DateNaissance,
                        d.LieuNaissance,
                        d.Sexe,
                        d.DateDelivrance,
                        d.Pays,
                        d.CinPathFront,
                        d.CinPathBack,
                        d.IndicateurResidencePath,
                        d.Telephone,
                        d.RelationBanque
                    })
                    .ToListAsync();

                return Ok(demandes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur lors du chargement des demandes.",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // GET: api/DemandeComptes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDemandeCompte(int id)
        {
            var demandeCompte = await _context.DemandeComptes
                .FirstOrDefaultAsync(d => d.Id == id);

            if (demandeCompte == null)
                return NotFound("Demande introuvable");

            return Ok(demandeCompte);
        }

        // POST: api/DemandeComptes/create-account-with-pdf
        [HttpPost("create-account-with-pdf")]
        public async Task<IActionResult> PostDemandeCompteAndMail([FromBody] DemandeCompte demandeCompte)
        {
            if (demandeCompte == null)
                return BadRequest("Données invalides");

            if (demandeCompte.IdClient == null)
                return BadRequest("IdClient est obligatoire");

            var client = await _context.Clients.FindAsync(demandeCompte.IdClient);
            if (client == null)
                return NotFound("Client non trouvé");

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.ClientId == demandeCompte.IdClient);
            if (profile == null)
                return NotFound("Profil (email) non trouvé");

            demandeCompte.Statut = "pending";
            demandeCompte.DateEnvoi = DateOnly.FromDateTime(DateTime.Now);

            _context.DemandeComptes.Add(demandeCompte);
            await _context.SaveChangesAsync();

            try
            {
                demandeCompte.DemandePdfPath = await _pdfService.GenerateAndSavePdfAsyncV2(client, demandeCompte);
                await _context.SaveChangesAsync();

                await _emailService.SendDemandeEmailAsync(profile.Email, client, demandeCompte.DemandePdfPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur PDF/Email",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }

            return Ok(demandeCompte);
        }

        // PATCH: api/DemandeComptes/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest("Status is required");

            var demande = await _context.DemandeComptes.FindAsync(id);
            if (demande == null)
                return NotFound("Demande introuvable");

            demande.Statut = dto.Status.ToLower();
            await _context.SaveChangesAsync();

            return Ok(demande);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveDemande(int id, [FromBody] ApproveDto? dto)
        {
            try
            {
                var demande = await _context.DemandeComptes.FindAsync(id);
                if (demande == null)
                    return NotFound("Demande introuvable");

                if (demande.IdAgence == null)
                    return BadRequest("IdAgence est null");

                Client client;

                if (demande.IdClient != null)
                {
                    // Client existant
                    client = await _context.Clients.FindAsync(demande.IdClient);
                    if (client == null)
                        return NotFound("Client référencé introuvable");

                    // Mise à jour des documents si présents
                    if (!string.IsNullOrEmpty(demande.CinPathFront))
                        client.CinPathFront = demande.CinPathFront;
                    if (!string.IsNullOrEmpty(demande.CinPathBack))
                        client.CinPathBack = demande.CinPathBack;
                    if (!string.IsNullOrEmpty(demande.IndicateurResidencePath))
                        client.IndicateurResidencePath = demande.IndicateurResidencePath;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Nouveau client avec tous ses documents
                    client = new Client
                    {
                        Nom = demande.Nom,
                        Prenom = demande.Prenom,
                        NumTel = demande.Telephone,
                        Adresse = demande.Adresse,
                        Ville = demande.Ville,
                        Pays = demande.Pays,
                        CodePostal = demande.CodePostal,
                        Profession = demande.Profession,
                        NomEmployeur = demande.NomEmployeur,
                        Devise = demande.Devise,
                        MontantRevMensuelNet = demande.RevenuMensuel,
                        Cin = demande.Cin,
                        DateNaissance = demande.DateNaissance,
                        LieuNaissance = demande.LieuNaissance,
                        Sexe = demande.Sexe,
                        DateDelivrance = demande.DateDelivrance,
                        RelationBanque = demande.RelationBanque,
                        CinPathFront = demande.CinPathFront,           // ✅
                        CinPathBack = demande.CinPathBack,             // ✅
                        IndicateurResidencePath = demande.IndicateurResidencePath // ✅
                    };
                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();
                }

                // Créer le nouveau compte
                await _context.Database.ExecuteSqlRawAsync(@"
        INSERT INTO compte (id_client, type_compte, date_ouverture, devise_compte, solde, solde_disponible, id_agence, pack, rib)
VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
      client.Id,
      demande.TypeCompte ?? "",
      DateOnly.FromDateTime(DateTime.UtcNow),
      demande.Devise ?? "",
      0m,
      0m,
      demande.IdAgence.Value,
      dto?.Pack,
      dto?.Rib ?? ""
  );

                // Mettre à jour la demande
                await _context.Database.ExecuteSqlRawAsync(@"
            UPDATE demande_compte SET id_client = {0}, statut = {1}
            WHERE id = {2}",
                    client.Id,
                    "approved",
                    id
                );

                return Ok(new { message = "Compte créé avec succès", clientId = client.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur lors de l'approbation",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
        // GET: api/DemandeComptes/client/5
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<DemandeCompte>>> GetDemandesByClient(int clientId)
        {
            var demandes = await _context.DemandeComptes
                .Where(d => d.IdClient == clientId)
                .Include(d => d.IdClientNavigation).Include(d => d.IdAgenceNavigation)
                .ToListAsync();

            if (!demandes.Any())
                return NotFound();

            return Ok(demandes);
        }

        // POST: api/DemandeComptes/5/reject
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectDemande(int id)
        {
            var demande = await _context.DemandeComptes.FindAsync(id);
            if (demande == null)
                return NotFound("Demande introuvable");

            demande.Statut = "rejected";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Demande rejetée" });
        }

        // DELETE: api/DemandeComptes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDemandeCompte(int id)
        {
            var demandeCompte = await _context.DemandeComptes.FindAsync(id);
            if (demandeCompte == null)
                return NotFound("Demande introuvable");

            _context.DemandeComptes.Remove(demandeCompte);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("admin/repair-paths")]
        public async Task<IActionResult> RepairPaths()
        {
            var demandes = await _context.DemandeComptes
                .Where(d => !string.IsNullOrEmpty(d.Reference))
                .ToListAsync();

            int count = 0;
            var extensions = new[] { ".jpg", ".jpeg", ".png" };

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");

            foreach (var d in demandes)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "Demandes", d.Reference!);
                if (!Directory.Exists(folder)) continue;

                bool updated = false;

                foreach (var ext in extensions)
                {
                    var front = Path.Combine(folder, $"cin_front{ext}");
                    var back = Path.Combine(folder, $"cin_back{ext}");
                    var residence = Path.Combine(folder, $"residence{ext}");

                    if (System.IO.File.Exists(front)) { d.CinPathFront = front; updated = true; }
                    if (System.IO.File.Exists(back)) { d.CinPathBack = back; updated = true; }
                    if (System.IO.File.Exists(residence)) { d.IndicateurResidencePath = residence; updated = true; }
                }

                if (updated) count++;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Paths repaired", count });
        }
        // GET: api/DemandeComptes/agence/5
        [HttpGet("agence/{idAgence}")]
        public async Task<IActionResult> GetDemandesByAgence(int idAgence)
        {
            try
            {
                var demandes = await _context.DemandeComptes
                    .Where(d => d.IdAgence == idAgence)
                    .Select(d => new
                    {
                        d.Id,
                        d.IdClient,
                        d.IdAgence,
                        d.DateEnvoi,
                        d.TypeCompte,
                        d.Etat,
                        d.DemandePdfPath,
                        d.Reference,
                        d.Statut,
                        d.Adresse,
                        d.Gouvernorat,
                        d.Civilite,
                        d.Ville,
                        d.CodePostal,
                        d.Profession,
                        d.NomEmployeur,
                        d.Devise,
                        d.RevenuMensuel,
                        d.Nom,
                        d.Prenom,
                        d.Cin,
                        d.DateNaissance,
                        d.LieuNaissance,
                        d.Sexe,
                        d.DateDelivrance,
                        d.Pays,
                        d.CinPathFront,
                        d.CinPathBack,
                        d.IndicateurResidencePath,
                        d.Telephone,
                        d.RelationBanque
                    })
                    .ToListAsync();

                return Ok(demandes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur lors du chargement des demandes par agence.",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }

    // DTO pour PATCH status
    public class StatusUpdateDto
    {
        public string? Status { get; set; }
    }
    public class ApproveDto
    {
        public int? Pack { get; set; }
        public string? Rib { get; set; }
    }
}