using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
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
        public async Task<ActionResult<Garden>> UpdateGarden(Guid id, Garden updatedGarden)
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

        [HttpGet]
        public async Task<ActionResult<List<GardenDto>>> GetGardens()
        {
            var gardens = await _context.Gardens
                .Include(g => g.Plants)
                .Select(g => new GardenDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Plants = g.Plants.Select(p => p.Name).ToList()
                })
                .ToListAsync();

            return Ok(gardens);
        }

        [HttpPost("{gardenId}/plants/{plantId}")]
        public async Task<IActionResult> AddPlantToGarden(Guid gardenId, Guid plantId)
        {
            var garden = await _context.Gardens
                .Include(g => g.Plants)//ogrod razem z roslinami
                .FirstOrDefaultAsync(g => g.Id == gardenId);

            if (garden == null)
            {
                return NotFound("Garden not found.");
            }

            var plant = await _context.Plants.FindAsync(plantId);//pobiera sama rosline

            if (plant == null)
            {
                return NotFound("Plant not found.");
            }

            if (garden.Plants.Any(p => p.Id == plantId))//czy sa duplikaty?
            {
                return BadRequest("Plant already assigned to garden.");
            }

            garden.Plants.Add(plant);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}