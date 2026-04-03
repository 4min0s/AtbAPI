using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public DemandeComptesController(DigiBankContext context, PdfGenerationService pdfService, EmailService emailService)
        {
            _context = context;
            _pdfService = pdfService;
            _emailService = emailService;
        }

        // GET: api/DemandeComptes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DemandeCompte>>> GetDemandeComptes()
        {
            return await _context.DemandeComptes.ToListAsync();
        }

        // GET: api/DemandeComptes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DemandeCompte>> GetDemandeCompte(int id)
        {
            var demandeCompte = await _context.DemandeComptes.FindAsync(id);

            if (demandeCompte == null)
            {
                return NotFound();
            }

            return demandeCompte;
        }

        // PUT: api/DemandeComptes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDemandeCompte(int id, DemandeCompte demandeCompte)
        {
            if (id != demandeCompte.Id)
            {
                return BadRequest();
            }

            _context.Entry(demandeCompte).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DemandeCompteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DemandeComptes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DemandeCompte>> PostDemandeCompte(DemandeCompte demandeCompte)
        {
            _context.DemandeComptes.Add(demandeCompte);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDemandeCompte", new { id = demandeCompte.Id }, demandeCompte);
        }

        // GET: api/DemandeComptes/client/5
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<DemandeCompte>>> GetDemandesByClient(int clientId)
        {
            var demandes = await _context.DemandeComptes
                .Where(d => d.IdClient == clientId)
                .Include(d => d.IdClientNavigation)
                .ToListAsync();

            if (!demandes.Any())
                return NotFound();

            return Ok(demandes);
        }


        // DELETE: api/DemandeComptes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDemandeCompte(int id)
        {
            var demandeCompte = await _context.DemandeComptes.FindAsync(id);
            if (demandeCompte == null)
            {
                return NotFound();
            }

            _context.DemandeComptes.Remove(demandeCompte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("create-with-pdf")]
        public async Task<ActionResult<DemandeCompte>> PostDemandeCompteAndMail(DemandeCompte demandeCompte)
        {
            // 1. Get the client
            var client = await _context.Clients.FindAsync(demandeCompte.IdClient);
            if (client == null)
                return NotFound("Client not found");

            // 2. Get the profile email
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.ClientId == demandeCompte.IdClient);
            if (profile == null)
                return NotFound("Profile not found");

            // 3. Save demande first to get its Id
            _context.DemandeComptes.Add(demandeCompte);
            await _context.SaveChangesAsync();

            // 4. Generate PDF and save to disk
            demandeCompte.DemandePdfPath = await _pdfService.GenerateAndSavePdfAsync(client, demandeCompte);
            await _context.SaveChangesAsync();

            // 5. Send email with PDF attached
            await _emailService.SendDemandeEmailAsync(profile.Email, client, demandeCompte.DemandePdfPath);

            return CreatedAtAction("GetDemandeCompte", new { id = demandeCompte.Id }, demandeCompte);
        }
        
        
        [HttpPost("create-account-with-pdf")]
        public async Task<ActionResult<DemandeCompte>> PostDemandeCompteAndMailnotfirst(DemandeCompte demandeCompte)
        {
            // 1. Get the client
            var client = await _context.Clients.FindAsync(demandeCompte.IdClient);
            if (client == null)
                return NotFound("Client not found");

            // 2. Get the profile email
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.ClientId == demandeCompte.IdClient);
            if (profile == null)
                return NotFound("Profile not found");

            // 3. Save demande first to get its Id
            _context.DemandeComptes.Add(demandeCompte);
            await _context.SaveChangesAsync();

            // 4. Generate PDF and save to disk
            demandeCompte.DemandePdfPath = await _pdfService.GenerateAndSavePdfAsyncV2(client, demandeCompte);
            await _context.SaveChangesAsync();

            // 5. Send email with PDF attached
            await _emailService.SendDemandeEmailAsync(profile.Email, client, demandeCompte.DemandePdfPath);

            return CreatedAtAction("GetDemandeCompte", new { id = demandeCompte.Id }, demandeCompte);
        }


        private bool DemandeCompteExists(int id)
        {
            return _context.DemandeComptes.Any(e => e.Id == id);
        }
    }
}
