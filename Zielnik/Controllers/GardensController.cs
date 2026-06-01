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
        public async Task<ActionResult<Garden>> CreateGarden(Garden garden)
        {
            // Tworzenie nowego ogrodu w bazie danych
            _context.Gardens.Add(garden);
            await _context.SaveChangesAsync();
            return Ok(garden);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Garden>> UpdateGarden(Guid id, [FromBody] GardenUpdateDto updatedData)
        {
            // Aktualizacja nazwy ogrodu za pomocą obiektu DTO
            var garden = await _context.Gardens
                .Include(g => g.Plants)
                    .ThenInclude(up => up.Plant)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garden == null)
            {
                return NotFound("Nie znaleziono ogrodu.");
            }

            // Przypisanie nowej nazwy wpisanej przez użytkownika
            garden.Name = updatedData.Name;

            await _context.SaveChangesAsync();
            return Ok(garden);
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

        //[HttpPost("{gardenId}/plants/{plantId}")]
        //public async Task<IActionResult> AddPlantToGarden(Guid gardenId, Guid plantId)
        //{
        //    // Przypisywanie (sadzenie) rośliny w wybranym ogrodzie
        //    var garden = await _context.Gardens
        //        .Include(g => g.Plants)
        //        .FirstOrDefaultAsync(g => g.Id == gardenId);

        //    if (garden == null)
        //    {
        //        return NotFound("Garden not found.");
        //    }

        //    var plant = await _context.Plants.FindAsync(plantId);

        //    if (plant == null)
        //    {
        //        return NotFound("Plant not found.");
        //    }

        //    if (garden.Plants.Any(p => p.Id == plantId))
        //    {
        //        return BadRequest("Plant already assigned to garden.");
        //    }

        //    garden.Plants.Add(plant);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{gardenId}/plants/{plantId}")]
        //public async Task<IActionResult> RemovePlantFromGarden(Guid gardenId, Guid plantId)
        //{
        //    // Usuwanie konkretnej rośliny z wybranego ogrodu
        //    var garden = await _context.Gardens
        //        .Include(g => g.Plants)
        //        .FirstOrDefaultAsync(g => g.Id == gardenId);

        //    if (garden == null)
        //        return NotFound("Garden not found.");

        //    var plant = garden.Plants.FirstOrDefault(p => p.Id == plantId);

        //    if (plant == null)
        //        return NotFound("Plant not found in garden.");

        //    garden.Plants.Remove(plant);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
    }
}