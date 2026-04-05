using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditsController : ControllerBase
    {
        private readonly DigiBankContext _context;

        public CreditsController(DigiBankContext context)
        {
            _context = context;
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
            {
                return NotFound();
            }

            return credit;
        }

        // PUT: api/Credits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCredit(int id, Credit credit)
        {
            if (id != credit.Id)
            {
                return BadRequest();
            }

            _context.Entry(credit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreditExists(id))
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


        // GET: api/Credits/client/5
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<Credit>>> GetCreditsByClient(int clientId)
        {
            var credits = await _context.Credits
                .Where(c => c.IdClient == clientId).Include(c => c.Echeances).Include(c => c.IdCompteNavigation)
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Credit>> PostCredit(Credit credit)
        {
            _context.Credits.Add(credit);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCredit", new { id = credit.Id }, credit);
        }

        // DELETE: api/Credits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCredit(int id)
        {
            var credit = await _context.Credits.FindAsync(id);
            if (credit == null)
            {
                return NotFound();
            }

            _context.Credits.Remove(credit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CreditExists(int id)
        {
            return _context.Credits.Any(e => e.Id == id);
        }
    }
}
