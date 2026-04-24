using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly DigiBankContext _context;

        public AgentsController(DigiBankContext context)
        {
            _context = context;
        }

        // POST: api/Agents/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AgentLoginDto dto)
        {
            if (dto == null)
                return BadRequest("Données invalides");

            var agent = await _context.Agents
                .FirstOrDefaultAsync(a => a.Email == dto.Email);

            if (agent == null)
                return Unauthorized("Email incorrect");

            if (agent.MotDePasse != dto.MotDePasse)
                return Unauthorized("Mot de passe incorrect");

            return Ok(new
            {
                agent.Id,
                agent.Nom,
                agent.Prenom,
                agent.Email,
                agent.IdAgence
            });
        }

        // GET: api/Agents
        [HttpGet]
        public async Task<IActionResult> GetAgents()
        {
            var agents = await _context.Agents.ToListAsync();
            return Ok(agents);
        }
    }
    public class AgentLoginDto
    {
        public string Email { get; set; } = null!;
        public string MotDePasse { get; set; } = null!;
    }

}