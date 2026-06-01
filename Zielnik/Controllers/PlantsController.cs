using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;

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
        public async Task<ActionResult<List<PlantDto>>> GetPlants(
    [FromQuery] string? category)
        {
            var query = _context.Plants
                .Include(p => p.Categories)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p =>
                    p.Categories.Any(c =>
                        c.Name.ToLower() == category.ToLower()));
            }

            var plants = await query
                .Select(p => new PlantDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Species = p.Species,
                    WateringFrequencyDays = p.WateringFrequencyDays,
                    Categories = p.Categories
                        .Select(c => c.Name)
                        .ToList()
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
    .FirstOrDefaultAsync(p => p.Id == id);

            if (plant == null)
            {
                return NotFound("Nie znaleziono rośliny o podanym ID.");
            }


            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlantDto>> GetPlant(Guid id)
        {
            var plant = await _context.Plants
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plant == null)
                return NotFound();

            return Ok(new PlantDto
            {
                Id = plant.Id,
                Name = plant.Name,
                Species = plant.Species,
                WateringFrequencyDays = plant.WateringFrequencyDays,
                Categories = plant.Categories
                    .Select(c => c.Name)
                    .ToList()
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlant(
    Guid id,
    UpdatePlantDto dto)
        {
            var plant = await _context.Plants.FindAsync(id);

            if (plant == null)
                return NotFound();

            plant.Name = dto.Name;
            plant.Species = dto.Species;
            plant.WateringFrequencyDays = dto.WateringFrequencyDays;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}