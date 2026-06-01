using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public PlantsController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<PlantDto>>> GetPlants()
        {
            // Pobieranie wszystkich roślin wraz z ich relacjami
            var plants = await _context.Plants
                .Include(p => p.Gardens)
                .Include(p => p.Categories)
                .Select(p => new PlantDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Species = p.Species,
                    WateringFrequencyDays = p.WateringFrequencyDays,
                    Categories = p.Categories.Select(c => c.Name).ToList()
                })
                .ToListAsync();

            return Ok(plants);
        }

        [HttpPost]
        public async Task<ActionResult<Plant>> CreatePlant(Plant plant)
        {
            // Dodawanie nowej rośliny do bazy danych
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return Ok(plant);
        }

        [HttpPost("{plantId}/categories/{categoryId}")]
        public async Task<IActionResult> AddCategoryToPlant(Guid plantId, Guid categoryId)
        {
            // Przypisywanie kategorii do istniejącej rośliny
            var plant = await _context.Plants
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == plantId);

            if (plant == null)
            {
                return NotFound("Plant not found.");
            }

            var category = await _context.PlantCategories.FindAsync(categoryId);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            if (plant.Categories.Any(c => c.Id == categoryId))
            {
                return BadRequest("Category already assigned to plant.");
            }

            plant.Categories.Add(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // METOD USUNIĘCIA ROŚLINY (DODANY FIX)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(Guid id)
        {
            // Wyszukiwanie rośliny w bazie danych wraz z ogrodami, do których jest przypisana
            var plant = await _context.Plants
                .Include(p => p.Gardens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plant == null)
            {
                return NotFound("Nie znaleziono rośliny o podanym ID.");
            }

            // Bezpieczne usuwanie powiązań z ogrodami przed usunięciem samej rośliny
            if (plant.Gardens != null && plant.Gardens.Any())
            {
                plant.Gardens.Clear();
            }

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
