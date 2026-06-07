using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;
using System.Security.Claims;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserPlantsController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public UserPlantsController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<UserPlant>> CreateUserPlant(CreateUserPlantDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var garden = await _context.Gardens
                .FirstOrDefaultAsync(g => g.Id == dto.GardenId && g.UserId == userId);

            if (garden == null)
                return BadRequest("Garden not found");

            var userPlant = new UserPlant
            {
                Id = Guid.NewGuid(),
                PlantId = dto.PlantId,
                GardenId = dto.GardenId,
                SowingDate = dto.SowingDate,
                PlantingDate = dto.PlantingDate,
                Nickname = dto.Nickname,
                Status = PlantStatus.Active,
                HarvestReminderDays = dto.HarvestReminderDays

            };

            _context.UserPlants.Add(userPlant);
            await _context.SaveChangesAsync();

            return Ok(userPlant);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPlants()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plants = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .Where(up => up.Garden.UserId == userId)
                .Select(up => new
                {
                    up.Id,
                    up.Nickname,
                    up.Status,
                    up.PlantingDate,
                    up.SowingDate,
                    up.GardenId,
                    PlantName = up.Plant.Name,
                    GardenName = up.Garden.Name
                })
                .ToListAsync();

            return Ok(plants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserPlant(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plant = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == id && up.Garden.UserId == userId);

            if (plant == null)
                return NotFound();

            return Ok(new
            {
                plant.Id,
                plant.Nickname,
                plant.Status,
                plant.PlantingDate,
                plant.SowingDate,
                PlantName = plant.Plant.Name,
                PlantSpecies = plant.Plant.Species,
                GardenName = plant.Garden.Name
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserPlant(Guid id, UpdateUserPlantDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plant = await _context.UserPlants
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == id && up.Garden.UserId == userId);

            if (plant == null)
                return NotFound();

            plant.Nickname = dto.Nickname;
            plant.SowingDate = dto.SowingDate;
            plant.PlantingDate = dto.PlantingDate;
            plant.Status = dto.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserPlant(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plant = await _context.UserPlants
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == id && up.Garden.UserId == userId);

            if (plant == null)
                return NotFound();

            _context.UserPlants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/timeline")]
        public async Task<IActionResult> GetTimeline(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plant = await _context.UserPlants
                .Include(up => up.Garden)
                .Include(up => up.Notes)
                .Include(up => up.Treatments)
                .Include(up => up.Harvests)
                .Include(up => up.Photos)
                .FirstOrDefaultAsync(up => up.Id == id && up.Garden.UserId == userId);

            if (plant == null) return NotFound();

            var timeline = new List<TimelineItemDto>();
            timeline.AddRange(plant.Notes.Select(n => new TimelineItemDto { Type = "Note", Title = n.Title, Date = n.CreatedAt }));
            timeline.AddRange(plant.Treatments.Select(t => new TimelineItemDto { Type = "Treatment", Title = t.TreatmentType, Date = t.PerformedAt }));
            timeline.AddRange(plant.Harvests.Select(h => new TimelineItemDto { Type = "Harvest", Title = $"{h.Quantity} {h.Unit}", Date = h.HarvestDate }));
            timeline.AddRange(plant.Photos.Select(p => new TimelineItemDto { Type = "Photo", Title = p.FilePath, Date = p.CreatedAt }));

            return Ok(timeline.OrderByDescending(t => t.Date).ToList());
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var plant = await _context.UserPlants
                .Include(up => up.Garden)
                .Include(up => up.Notes)
                .Include(up => up.Treatments)
                .Include(up => up.Harvests)
                .Include(up => up.Photos)
                .FirstOrDefaultAsync(up => up.Id == id && up.Garden.UserId == userId);

            if (plant == null) return NotFound();

            return Ok(new UserPlantSummaryDto
            {
                NotesCount = plant.Notes.Count,
                TreatmentsCount = plant.Treatments.Count,
                HarvestsCount = plant.Harvests.Count,
                PhotosCount = plant.Photos.Count,
                TotalHarvest = plant.Harvests.Sum(h => h.Quantity),
                LastHarvestDate = plant.Harvests.Any() ? plant.Harvests.Max(h => h.HarvestDate) : null
            });
        }

        [HttpGet("{id}/statistics")]
        public async Task<IActionResult> GetStatistics(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var plant = await _context.UserPlants
                .Include(up => up.Garden)
                .Include(up => up.Treatments)
                .Include(up => up.Harvests)
                .FirstOrDefaultAsync(up => up.Id == id && up.Garden.UserId == userId);

            if (plant == null) return NotFound();

            return Ok(new UserPlantStatisticsDto
            {
                DaysSincePlanting = plant.PlantingDate.HasValue ? (DateTime.UtcNow.Date - plant.PlantingDate.Value.Date).Days : 0,
                TreatmentsCount = plant.Treatments.Count,
                HarvestsCount = plant.Harvests.Count,
                TotalHarvest = plant.Harvests.Sum(h => h.Quantity)
            });
        }


    }
}