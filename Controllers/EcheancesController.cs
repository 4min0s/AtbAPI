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
    public class EcheancesController : ControllerBase
    {
        private readonly DigiBankContext _context;

        public EcheancesController(DigiBankContext context)
        {
            _context = context;
        }

        // GET: api/Echeances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Echeance>>> GetEcheances()
        {
            return await _context.Echeances.ToListAsync();
        }

        // GET: api/Echeances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Echeance>> GetEcheance(int id)
        {
            var echeance = await _context.Echeances.FindAsync(id);

            if (echeance == null)
            {
                return NotFound();
            }

            return echeance;
        }

        // PUT: api/Echeances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEcheance(int id, Echeance echeance)
        {
            if (id != echeance.Id)
            {
                return BadRequest();
            }

            _context.Entry(echeance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EcheanceExists(id))
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

        // POST: api/Echeances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Echeance>> PostEcheance(Echeance echeance)
        {
            _context.Echeances.Add(echeance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEcheance", new { id = echeance.Id }, echeance);
        }

        // DELETE: api/Echeances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEcheance(int id)
        {
            var echeance = await _context.Echeances.FindAsync(id);
            if (echeance == null)
            {
                return NotFound();
            }

            _context.Echeances.Remove(echeance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EcheanceExists(int id)
        {
            return _context.Echeances.Any(e => e.Id == id);
        }
    }
}
