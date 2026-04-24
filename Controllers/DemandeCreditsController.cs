using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemandeCreditsController : ControllerBase
    {
        private readonly DigiBankContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public DemandeCreditsController(DigiBankContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
        }

        // GET: api/DemandeCredits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DemandeCredit>>> GetDemandeCredits()
        {
            return await _context.DemandeCredits
                .Include(d => d.DemandeCreditVehicule)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierAcquisition)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierConstruction)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierRenovation)
                .ToListAsync();
        }

        // GET: api/DemandeCredits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DemandeCredit>> GetDemandeCredit(int id)
        {
            var demande = await _context.DemandeCredits
                .Include(d => d.DemandeCreditVehicule)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierAcquisition)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierConstruction)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierRenovation)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (demande == null) return NotFound();
            return demande;
        }

        // GET: api/DemandeCredits/client/5
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<DemandeCredit>>> GetByClient(int clientId)
        {
            var demandes = await _context.DemandeCredits
                .Where(d => d.IdClient == clientId)
                .Include(d => d.DemandeCreditVehicule)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierAcquisition)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierConstruction)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierRenovation)
                .ToListAsync();

            if (!demandes.Any()) return NotFound();
            return Ok(demandes);
        }
        [HttpPost]
        public async Task<ActionResult<DemandeCredit>> PostDemandeCredit(DemandeCreditDto dto)
        {
            // ── Validate type ──────────────────────────────────────────────
            var validTypes = new[] { "consommation", "vehicule", "immobilier" };
            if (!validTypes.Contains(dto.TypeCredit.ToLower()))
                return BadRequest("TypeCredit must be: consommation, vehicule, or immobilier");

            // ── 1. Create the parent demande_credit row ────────────────────
            var demande = new DemandeCredit
            {
                IdClient = dto.IdClient,
                IdCompte = dto.IdCompte,
                TypeCredit = dto.TypeCredit.ToLower(),
                NatureCredit = dto.NatureCredit,
                Objet = dto.Objet,
                Montant = dto.Montant,
                DureeMois = dto.DureeMois,
                Periodicite = dto.Periodicite,
                DureeGrace = dto.DureeGrace ?? 0,
                Pays = dto.Pays,
                Adresse = dto.Adresse,
                Ville = dto.Ville,
                CodePostal = dto.CodePostal,
                Gouvernorat = dto.Gouvernorat,
                Etat = 0,
                DateEnvoi = DateOnly.FromDateTime(DateTime.Today),
                Reference = GenerateReference(),
                SituationProfessionnelle = dto.SituationProfessionnelle,
                RevenuMensuelNet = dto.RevenuMensuelNet,
                AutresSourcesDeRevenu = dto.AutresSourcesDeRevenu,
                MontantMensuelAutresRevenus = dto.MontantMensuelAutresRevenus,
                CinPathFront = dto.CinPathFront,
                CinPathBack = dto.CinPathBack,
                IndicateurResidencePath = dto.IndicateurResidencePath,
                FicheDePaiePath = dto.FicheDePaiePath,
                AttestationDeTravailPath = dto.AttestationDeTravailPath,
                AttestationDeSalairePath = dto.AttestationDeSalairePath,
            };

            _context.DemandeCredits.Add(demande);
            await _context.SaveChangesAsync();

            // ── 2. Create the child row based on type ──────────────────────
            switch (dto.TypeCredit.ToLower())
            {
                case "vehicule":
                    if (dto.VehiculeNeuf == null)
                        return BadRequest("VehiculeNeuf is required for type vehicule");

                    _context.DemandeCreditVehicules.Add(new DemandeCreditVehicule
                    {
                        IdDemande = demande.Id,
                        PrixVehicule = dto.PrixVehicule,
                        PuissanceFiscale = dto.PuissanceFiscale,
                        VehiculeNeuf = dto.VehiculeNeuf.Value,
                        FactureProformaPath = dto.VehiculeNeuf.Value ? dto.FactureProformaPath : null,
                        DatePremiereMiseEnCirculation = !dto.VehiculeNeuf.Value ? dto.DatePremiereMiseEnCirculation : null,
                        CarteGrisePath = !dto.VehiculeNeuf.Value ? dto.CarteGrisePath : null,
                        PromesseVentePath = !dto.VehiculeNeuf.Value ? dto.PromesseVentePath : null,
                    });
                    break;

                case "immobilier":
                    var validSousTypes = new[] { "acquisition", "construction", "renovation" };
                    if (string.IsNullOrEmpty(dto.SousType) || !validSousTypes.Contains(dto.SousType.ToLower()))
                        return BadRequest("SousType must be: acquisition, construction, or renovation");

                    var immobilier = new DemandeCreditImmobilier
                    {
                        IdDemande = demande.Id,
                        SousType = dto.SousType.ToLower(),
                    };
                    _context.DemandeCreditImmobiliers.Add(immobilier);
                    await _context.SaveChangesAsync();

                    switch (dto.SousType.ToLower())
                    {
                        case "acquisition":
                            _context.DemandeCreditImmobilierAcquisitions.Add(new DemandeCreditImmobilierAcquisition
                            {
                                IdImmobilier = immobilier.Id,
                                PrixImmobilier = dto.PrixImmobilier,
                                TypeAcquisition = dto.TypeAcquisition,
                                PromesseVentePath = dto.PromesseVenteImmobilierPath,
                            });
                            break;

                        case "construction":
                            _context.DemandeCreditImmobilierConstructions.Add(new DemandeCreditImmobilierConstruction
                            {
                                IdImmobilier = immobilier.Id,
                                CoutTravaux = dto.CoutTravaux,
                                DevisEstimatifPath = dto.DevisEstimatifPath,
                                AutorisationBatirPath = dto.AutorisationBatirPath,
                                PlanArchitectePath = dto.PlanArchitectePath,
                            });
                            break;

                        case "renovation":
                            _context.DemandeCreditImmobilierRenovations.Add(new DemandeCreditImmobilierRenovation
                            {
                                IdImmobilier = immobilier.Id,
                                CoutTravaux = dto.CoutTravauxRenovation,
                                DevisEstimatifPath = dto.DevisEstimatifRenovationPath,
                            });
                            break;
                    }
                    break;

                    // consommation: nothing extra to create
            }

            await _context.SaveChangesAsync();

            // ── 3. Fire and forget — PDF + Email ──────────────────────────
            var demandeId = demande.Id;
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<DigiBankContext>();
                    var pdfService = scope.ServiceProvider.GetRequiredService<PdfGenerationService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    var fullDemande = await context.DemandeCredits
                        .Include(d => d.DemandeCreditVehicule)
                        .Include(d => d.DemandeCreditImmobilier)
                            .ThenInclude(i => i!.DemandeCreditImmobilierAcquisition)
                        .Include(d => d.DemandeCreditImmobilier)
                            .ThenInclude(i => i!.DemandeCreditImmobilierConstruction)
                        .Include(d => d.DemandeCreditImmobilier)
                            .ThenInclude(i => i!.DemandeCreditImmobilierRenovation)
                        .Include(d => d.IdClientNavigation)
                        .Include(d => d.IdCompteNavigation)
                            .ThenInclude(c => c!.IdAgenceNavigation)
                        .FirstOrDefaultAsync(d => d.Id == demandeId);

                    if (fullDemande?.IdClientNavigation == null || fullDemande.IdCompteNavigation == null)
                        return;

                    // ── Generate PDF ───────────────────────────────────────
                    await pdfService.GenerateDemandeCreditPdfAsync(
                        fullDemande,
                        fullDemande.IdClientNavigation,
                        fullDemande.IdCompteNavigation
                    );

                    // ── Send email with PDF attached ───────────────────────
                    var pdfPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Documents", "DemandesCredit",
                        fullDemande.Reference ?? fullDemande.Id.ToString(),
                        "demande_credit.pdf"
                    );

                    var profile = await context.Profiles
                        .FirstOrDefaultAsync(p => p.ClientId == fullDemande.IdClient);

                    if (profile != null && System.IO.File.Exists(pdfPath))
                    {
                        await emailService.SendDemandeCreditEmailAsync(
                            profile.Email,
                            fullDemande.IdClientNavigation,
                            pdfPath,
                            fullDemande
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Background Task Error] {ex.Message}");
                }
            });

            // ── 4. Return immediately — don't wait for PDF or email ────────
            return CreatedAtAction(
                nameof(GetDemandeCredit),
                new { id = demande.Id },
                await GetFullDemande(demande.Id)
            );
        }


        // PUT: api/DemandeCredits/5/etat  (for admin to change status)
        [HttpPut("{id}/etat")]
        public async Task<IActionResult> UpdateEtat(int id, [FromBody] int etat)
        {
            var demande = await _context.DemandeCredits.FindAsync(id);
            if (demande == null) return NotFound();

            demande.Etat = etat;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/DemandeCredits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDemandeCredit(int id)
        {
            var demande = await _context.DemandeCredits.FindAsync(id);
            if (demande == null) return NotFound();

            _context.DemandeCredits.Remove(demande);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private async Task<DemandeCredit?> GetFullDemande(int id)
        {
            return await _context.DemandeCredits
                .Include(d => d.DemandeCreditVehicule)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierAcquisition)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierConstruction)
                .Include(d => d.DemandeCreditImmobilier)
                    .ThenInclude(i => i!.DemandeCreditImmobilierRenovation)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        private static string GenerateReference()
        {
            return $"DMD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        }

        private bool DemandeCreditExists(int id)
        {
            return _context.DemandeCredits.Any(e => e.Id == id);
        }
    }
}