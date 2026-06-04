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
    [Authorize]
    public class PlantsController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public PlantsController(ZielnikDbContext context)
        {
            _context = context;
        }

        // GET ALL PLANTS
        [HttpGet]
        public async Task<ActionResult<List<PlantDto>>> GetPlants()
        {
            var plants = await _context.Plants
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

        // CREATE PLANT (DTO)
        [HttpPost]
        public async Task<ActionResult> CreatePlant([FromBody] PlantDto dto)
        {
            var plant = new Plant
            {
                Name = dto.Name,
                Species = dto.Species,
                WateringFrequencyDays = dto.WateringFrequencyDays
            };

            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return Ok(plant);
        }

        // ADD CATEGORY TO PLANT
        [HttpPost("{plantId}/categories/{categoryId}")]
        public async Task<IActionResult> AddCategoryToPlant(Guid plantId, Guid categoryId)
        {
            var plant = await _context.Plants
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == plantId);

            if (plant == null)
                return NotFound("Plant not found");

            var category = await _context.PlantCategories.FindAsync(categoryId);

            if (category == null)
                return NotFound("Category not found");

            if (plant.Categories.Any(c => c.Id == categoryId))
                return BadRequest("Category already assigned");

            plant.Categories.Add(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE PLANT
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(Guid id)
        {
            var plant = await _context.Plants
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plant == null)
                return NotFound("Plant not found");

            plant.Categories.Clear();
           

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}