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

        // POST: api/DemandeComptes/5/approve
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveDemande(int id)
        {
            try
            {
                var demande = await _context.DemandeComptes.FindAsync(id);
                if (demande == null)
                    return NotFound("Demande introuvable");

                if (demande.IdAgence == null)
                    return BadRequest("IdAgence est null");

                // créer client
                var client = new Client
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
                    RelationBanque = demande.RelationBanque
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                // créer compte
                var compte = new Compte
                {
                    IdClient = client.Id,
                    TypeCompte = demande.TypeCompte,
                    DateOuverture = DateTime.UtcNow,
                    DeviseCompte = demande.Devise,
                    Solde = 0,
                    SoldeDisponible = 0,
                    IdAgence = demande.IdAgence.Value
                };

                _context.Comptes.Add(compte);

                // mise à jour demande
                demande.IdClient = client.Id;
                demande.Statut = "approved";

                await _context.SaveChangesAsync();

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
    }

    // DTO pour PATCH status
    public class StatusUpdateDto
    {
        public string? Status { get; set; }
    }
}