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
    public class ComptesController : ControllerBase
    {
        private readonly DigiBankContext _context;

        public ComptesController(DigiBankContext context)
        {
            _context = context;
        }

        // GET: api/Comptes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compte>>> GetComptes()
        {

            return await _context.Comptes.Include(c => c.Credits).ToListAsync();
        }

        // GET: api/Comptes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Compte>> GetCompte(int id)
        {
            var compte = await _context.Comptes.FindAsync(id);

            if (compte == null)
            {
                return NotFound();
            }

            return compte;
        }

        // PUT: api/Comptes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompte(int id, Compte compte)
        {
            if (id != compte.Id)
            {
                return BadRequest();
            }

            _context.Entry(compte).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompteExists(id))
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

        // GET: api/Comptes/client/5
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<Compte>>> GetComptesByClient(int clientId)
        {
            var comptes = await _context.Comptes
                .Where(c => c.IdClient == clientId).Include(c => c.Credits).Include(d => d.IdAgenceNavigation)
                .ToListAsync();

            if (!comptes.Any())
                return NotFound();

            return Ok(comptes);
        }

        // POST: api/Comptes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Compte>> PostCompte(Compte compte)
        {
            _context.Comptes.Add(compte);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompte", new { id = compte.Id }, compte);
        }

        // DELETE: api/Comptes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompte(int id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
            {
                return NotFound();
            }

            _context.Comptes.Remove(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompteExists(int id)
        {
            return _context.Comptes.Any(e => e.Id == id);
        }
    }
}
