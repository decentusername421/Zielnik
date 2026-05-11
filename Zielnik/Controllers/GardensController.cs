using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.Entities;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GardensController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public GardensController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Garden>> GetGarden(Guid id)
        {
            var garden = await _context.Gardens.FindAsync(id);

            if (garden == null)
            {
                return NotFound();
            }

            return Ok(garden);
        }

        [HttpPost]
        public async Task<ActionResult<Garden>> CreateGarden(Garden garden)
        {
            _context.Gardens.Add(garden);

            await _context.SaveChangesAsync();

            return Ok(garden);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Garden>> UpdateGarden(
    Guid id,
    Garden updatedGarden)
        {
            var garden = await _context.Gardens.FindAsync(id);

            if (garden == null)
            {
                return NotFound();
            }

            garden.Name = updatedGarden.Name;

            await _context.SaveChangesAsync();

            return Ok(garden);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGarden(Guid id)
        {
            var garden = await _context.Gardens.FindAsync(id);

            if (garden == null)
            {
                return NotFound();
            }

            _context.Gardens.Remove(garden);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}