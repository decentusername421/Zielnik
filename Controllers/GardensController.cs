using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GardensController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public GardensController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GardenDto>> GetGarden(Guid id)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var garden = await _context.Gardens
                .Include(g => g.Plants)
                .ThenInclude(up => up.Plant)
                .FirstOrDefaultAsync(g =>
                    g.Id == id &&
                    g.UserId == userId);

            if (garden == null)
            {
                return NotFound();
            }

            var gardenDto = new GardenDto
            {
                Id = garden.Id,
                Name = garden.Name,
                Plants = garden.Plants
                    .Select(p => p.Plant.Name)
                    .ToList()
            };

            return Ok(gardenDto);
        }


        [HttpPost]
        public async Task<ActionResult<GardenDto>> CreateGarden(CreateGardenDto dto)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var garden = new Garden
            {
                Name = dto.Name,
                UserId = userId
            };

            _context.Gardens.Add(garden);
            await _context.SaveChangesAsync();

            var gardenDto = new GardenDto
            {
                Id = garden.Id,
                Name = garden.Name,
                Plants = new List<string>()
            };

            return CreatedAtAction(
                nameof(GetGarden),
                new { id = garden.Id },
                gardenDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GardenDto>> UpdateGarden(
     Guid id,
     [FromBody] UpdateGardenDto updatedData)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var garden = await _context.Gardens
                .Include(g => g.Plants)
                .ThenInclude(up => up.Plant)
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (garden == null)
            {
                return NotFound("Nie znaleziono ogrodu.");
            }

            garden.Name = updatedData.Name;

            await _context.SaveChangesAsync();

            var gardenDto = new GardenDto
            {
                Id = garden.Id,
                Name = garden.Name,
                Plants = garden.Plants
                    .Select(p => p.Plant.Name)
                    .ToList()
            };

            return Ok(gardenDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGarden(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var garden = await _context.Gardens
                .Include(g => g.Plants)
                .ThenInclude(up => up.Plant)
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

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
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var gardens = await _context.Gardens
                .Where(g => g.UserId == userId)
                .Include(g => g.Plants)
                .ThenInclude(up => up.Plant)
                .Select(g => new GardenDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Plants = g.Plants
                        .Select(p => p.Plant.Name)
                        .ToList()
                })
                .ToListAsync();

            return Ok(gardens);
        }

    }
}
