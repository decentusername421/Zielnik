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
        public async Task<ActionResult<GardenDto>> GetGarden(Guid id)
        {
            // Pobieranie szczegółów jednego ogrodu wraz z roślinami
            var garden = await _context.Gardens
                .Include(g => g.Plants)
                .ThenInclude(up => up.Plant)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garden == null)
            {
                return NotFound();
            }

            // Mapowanie na bezpieczny format GardenDto
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
            var garden = new Garden
            {
                Name = dto.Name
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
            var garden = await _context.Gardens
                .Include(g => g.Plants)
                .ThenInclude(up => up.Plant)
                .FirstOrDefaultAsync(g => g.Id == id);

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
            // Usuwanie ogrodu z bazy danych
            var garden = await _context.Gardens
                .Include(g => g.Plants)
                 .ThenInclude(up => up.Plant)
                .FirstOrDefaultAsync(g => g.Id == id);

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
            // Pobieranie listy wszystkich ogrodów dla strony głównej
            var gardens = await _context.Gardens
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