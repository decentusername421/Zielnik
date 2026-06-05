using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


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
        public async Task<ActionResult<List<PlantDto>>> GetPlants(
    [FromQuery] string? category)
        {
            var query = _context.Plants
    .Where(p => p.IsApproved)
    .Include(p => p.Categories)
    .Include(p => p.UserPlants)
        .ThenInclude(up => up.Garden)
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

        // CREATE PLANT (DTO)
        [HttpPost]
        public async Task<ActionResult> CreatePlant([FromBody] CreatePlantDto dto)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var isAdmin = User.IsInRole("Admin");

            var plant = new Plant
            {
                Name = dto.Name,
                Species = dto.Species,
                WateringFrequencyDays = dto.WateringFrequencyDays,

                IsApproved = isAdmin,
                CreatedByUserId = isAdmin ? null : userId
            };

            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPlant),
                new { id = plant.Id },
                new PlantDto
                {
                    Id = plant.Id,
                    Name = plant.Name,
                    Species = plant.Species,
                    WateringFrequencyDays = plant.WateringFrequencyDays
                });
        }

        // ADD CATEGORY TO PLANT
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<PlantDto>> GetPlant(Guid id)
        {
            var plant = await _context.Plants
                .Include(p => p.Categories)
                .Include(p => p.UserPlants)
                .ThenInclude(up => up.Garden)
                .FirstOrDefaultAsync(p =>
                        p.Id == id &&
                        p.IsApproved);

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
                    .ToList(),

                Gardens = plant.UserPlants
                    .Select(up => up.Garden.Name)
                    .ToList()
            });
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPlants()
        {
            var plants = await _context.Plants
                .Where(p => !p.IsApproved)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Species,
                    p.CreatedByUserId
                })
                .ToListAsync();

            return Ok(plants);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApprovePlant(Guid id)
        {
            var plant = await _context.Plants
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plant == null)
                return NotFound();

            plant.IsApproved = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}