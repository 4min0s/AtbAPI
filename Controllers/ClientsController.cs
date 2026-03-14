using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly DigiBankContext _context;

        public ClientsController(DigiBankContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // PUT: api/Clients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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

        // POST: api/Clients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {

            // Check if CIN already exists
            var existing = await _context.Clients
                .FirstOrDefaultAsync(c => c.Cin == client.Cin);

            if (existing != null)  // ← this part is missing in your code
            {
                return Conflict("A client with this CIN already exists.");
            }
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        // PATCH: api/Clients/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchClient(int id, [FromBody] JsonElement updates)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            // Only update fields that are provided
            if (updates.TryGetProperty("adresse", out var adresse))
                client.Adresse = adresse.GetString();
            if (updates.TryGetProperty("pays", out var pays))
                client.Pays = pays.GetString();
            if (updates.TryGetProperty("gouvernorat", out var gouvernorat))
                client.Gouvernorat = gouvernorat.GetString();
            if (updates.TryGetProperty("ville", out var ville))
                client.Ville = ville.GetString();
            if (updates.TryGetProperty("codePostal", out var codePostal))
                client.CodePostal = codePostal.GetString();
            if (updates.TryGetProperty("nom", out var nom))
                client.Nom = nom.GetString();
            if (updates.TryGetProperty("prenom", out var prenom))
                client.Prenom = prenom.GetString();
            if (updates.TryGetProperty("profession", out var profession))
                client.Profession = profession.GetString();
            if (updates.TryGetProperty("nomEmployeur", out var nomEmployeur))
                client.NomEmployeur = nomEmployeur.GetString();
            if (updates.TryGetProperty("numTel", out var numTel))
                client.NumTel = numTel.GetString();
            if (updates.TryGetProperty("montantRevMensuelNet", out var montant))
                client.MontantRevMensuelNet = (decimal)montant.GetDouble();
            if (updates.TryGetProperty("devise", out var devise))
                client.Devise = devise.GetString();
            if (updates.TryGetProperty("relationBanque", out var relationBanque))
                client.RelationBanque = relationBanque.GetBoolean();


            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}
