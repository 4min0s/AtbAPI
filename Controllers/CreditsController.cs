using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditsController : ControllerBase
    {
        private readonly DigiBankContext _context;
        private readonly CreditSimulatorService _simulator;
        private readonly PdfGeneratorService _pdfGenerator;

        public CreditsController(
            DigiBankContext context,
            CreditSimulatorService simulator,
            PdfGeneratorService pdfGenerator)
        {
            _context = context;
            _simulator = simulator;
            _pdfGenerator = pdfGenerator;
        }

        // GET: api/Credits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Credit>>> GetCredits()
        {
            return await _context.Credits.Include(c => c.Echeances).ToListAsync();
        }

        // GET: api/Credits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Credit>> GetCredit(int id)
        {
            var credit = await _context.Credits.FindAsync(id);
            if (credit == null)
                return NotFound();
            return credit;
        }

        // PUT: api/Credits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCredit(int id, Credit credit)
        {
            if (id != credit.Id)
                return BadRequest();

            _context.Entry(credit).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreditExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        // GET: api/Credits/client/5
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<Credit>>> GetCreditsByClient(int clientId)
        {
            var credits = await _context.Credits
                .Where(c => c.IdClient == clientId)
                .Include(c=>c.CurrentEcheanceNavigation)
                .Include(c => c.IdCompteNavigation)
                .ToListAsync();

            if (!credits.Any())
                return NotFound();
            return Ok(credits);
        }

        // GET: api/Credits/compte/5
        [HttpGet("compte/{compteId}")]
        public async Task<ActionResult<IEnumerable<Credit>>> GetCreditsByCompte(int compteId)
        {
            var credits = await _context.Credits
                .Where(c => c.IdCompte == compteId)
                .ToListAsync();

            if (!credits.Any())
                return NotFound();
            return Ok(credits);
        }

        // POST: api/Credits
[HttpPost]
public async Task<ActionResult<Credit>> PostCredit(Credit credit)
{
    // 1. Save credit first to get a valid Id
    _context.Credits.Add(credit);
    await _context.SaveChangesAsync();

    // 2. Generate and save echeances to DB
    await _pdfGenerator.GenerateAndSaveEcheancesAsync(credit, _context);

    // 3. Reload credit with all navigation properties needed for PDF
    var creditWithIncludes = await _context.Credits
        .Include(c => c.Echeances)
        .Include(c => c.IdClientNavigation)
        .Include(c => c.IdCompteNavigation)
        .FirstOrDefaultAsync(c => c.Id == credit.Id);

            // 4. Set current_echeance to the first echeance
    var firstEcheance = creditWithIncludes!.Echeances
    .OrderBy(e => e.NumeroEcheance)
    .FirstOrDefault();

    if (firstEcheance != null)
    {
        firstEcheance.Etat = 1;  // ✅ safe now
        creditWithIncludes.CurrentEcheance = firstEcheance.Id;
        await _context.SaveChangesAsync();
    }

            if (firstEcheance != null)
    {
        creditWithIncludes.CurrentEcheance = firstEcheance.Id;
        await _context.SaveChangesAsync();
    }

    // 5. Generate PDF from the echeances
    var pdfPath = _pdfGenerator.GeneratePdf(creditWithIncludes!);

    // 6. Persist the PDF path on the credit record
    creditWithIncludes!.TabAmortissementPath = pdfPath;
    await _context.SaveChangesAsync();

    return CreatedAtAction("GetCredit", new { id = credit.Id }, creditWithIncludes);
}

        // DELETE: api/Credits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCredit(int id)
        {
            var credit = await _context.Credits.FindAsync(id);
            if (credit == null)
                return NotFound();

            _context.Credits.Remove(credit);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ── POST: api/Credits/simulate ─────────────────────────────────
        [HttpPost("simulate")]
        public ActionResult<SimulationResponseDto> Simulate([FromBody] SimulationRequestDto dto)
        {
            var result = _simulator.Simulate(
                dto.Montant,
                dto.TauxAnnuel,
                dto.Periodicite,
                dto.DureeGrace,
                dto.DurationMonths);

            return Ok(new SimulationResponseDto
            {
                NombreEcheances = result.NombreEcheances,
                TotalInteret = result.TotalInteret,
                TotalCapital = result.TotalCapital,
                CoutTotalCredit = result.CoutTotalCredit,
                echellance = result.echellance,
                dernierechellance = result.dernierechellance

            });
        }

       

        private bool CreditExists(int id)
        {
            return _context.Credits.Any(e => e.Id == id);
        }
    }
}