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
            var garden = await _context.Gardens
                .Include(g => g.Plants)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garden == null)
            {
                return NotFound();
            }

            return Ok(garden);
        }

        [HttpPost]
        public async Task<ActionResult<GardenDto>> CreateGarden(CreateGardenDto dto)
        {
            var garden = new Garden
            {
                Name = dto.Name
            };

            _context.Gardens.Add(garden);

            await _context.SaveChangesAsync();

            return Ok(new GardenDto
            {
                Id = garden.Id,
                Name = garden.Name,
                Plants = new List<string>()
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GardenDto>> UpdateGarden(
    Guid id,
    UpdateGardenDto dto)
        {
            var garden = await _context.Gardens
    .Include(g => g.Plants)
    .FirstOrDefaultAsync(g => g.Id == id);

            if (garden == null)
            {
                return NotFound();
            }

            garden.Name = dto.Name;

            await _context.SaveChangesAsync();

            return Ok(new GardenDto
            {
                Id = garden.Id,
                Name = garden.Name,
                Plants = garden.Plants.Select(p => p.Name).ToList()
            });
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

        [HttpDelete("{gardenId}/plants/{plantId}")]
        public async Task<IActionResult> RemovePlantFromGarden(
    Guid gardenId,
    Guid plantId)
        {
            var garden = await _context.Gardens
                .Include(g => g.Plants)
                .FirstOrDefaultAsync(g => g.Id == gardenId);

            if (garden == null)
            {
                return NotFound("Garden not found.");
            }

            var plant = garden.Plants
                .FirstOrDefault(p => p.Id == plantId);

            if (plant == null)
            {
                return NotFound("Plant not assigned to garden.");
            }

            garden.Plants.Remove(plant);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}