using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatsController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public StatsController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserStatisticsDto>> GetStats()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var gardens = await _context.Gardens
                .Where(g => g.UserId == userId)
                .Select(g => g.Id)
                .ToListAsync();

            var plants = await _context.UserPlants
                .Include(x => x.Notes)
                .Include(x => x.Treatments)
                .Include(x => x.Harvests)
                .Include(x => x.Photos)
                .Where(x => gardens.Contains(x.GardenId))
                .ToListAsync();

            var stats = new UserStatisticsDto
            {
                GardensCount = gardens.Count,

                PlantsCount = plants.Count,

                ActivePlantsCount = plants.Count(
                    p => p.Status == PlantStatus.Active),

                HarvestedPlantsCount = plants.Count(
                    p => p.Status == PlantStatus.Harvested),

                DeadPlantsCount = plants.Count(
                    p => p.Status == PlantStatus.Dead),

                NotesCount = plants.Sum(
                    p => p.Notes.Count),

                TreatmentsCount = plants.Sum(
                    p => p.Treatments.Count),

                WateringsCount = plants.Sum(
                    p => p.Treatments.Count(
                        t => t.TreatmentType == "Watering")),

                FertilizingsCount = plants.Sum(
                    p => p.Treatments.Count(
                        t => t.TreatmentType == "Fertilizing")),

                SprayingsCount = plants.Sum(
                    p => p.Treatments.Count(
                        t => t.TreatmentType == "Spraying")),

                HarvestsCount = plants.Sum(
                    p => p.Harvests.Count),

                TotalHarvest = plants.Sum(
                    p => p.Harvests.Sum(
                        h => h.Quantity)),

                PhotosCount = plants.Sum(
                    p => p.Photos.Count)
            };

            return Ok(stats);
        }

        [HttpGet("plant/{userPlantId}")]
        public async Task<ActionResult<PlantStatisticsDto>>
    GetPlantStats(Guid userPlantId)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var plant = await _context.UserPlants
                .Include(x => x.Plant)
                .Include(x => x.Garden)
                .Include(x => x.Notes)
                .Include(x => x.Treatments)
                .Include(x => x.Harvests)
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x =>
                    x.Id == userPlantId &&
                    x.Garden.UserId == userId);

            if (plant == null)
                return NotFound();

            var stats = new PlantStatisticsDto
            {
                UserPlantId = plant.Id,

                PlantName = plant.Plant.Name,

                Nickname = plant.Nickname,

                NotesCount = plant.Notes.Count,

                TreatmentsCount = plant.Treatments.Count,

                WateringsCount = plant.Treatments.Count(
                    t => t.TreatmentType == "Watering"),

                FertilizingsCount = plant.Treatments.Count(
                    t => t.TreatmentType == "Fertilizing"),

                SprayingsCount = plant.Treatments.Count(
                    t => t.TreatmentType == "Spraying"),

                HarvestsCount = plant.Harvests.Count,

                TotalHarvest = plant.Harvests.Sum(
                    h => h.Quantity),

                TotalFruits = plant.Harvests.Sum(
                    h => h.FruitsCount ?? 0),

                PhotosCount = plant.Photos.Count
            };

            return Ok(stats);
        }
    }
}