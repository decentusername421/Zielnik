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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                .AsSplitQuery()
                .ToListAsync();

            var stats = new UserStatisticsDto
            {
                GardensCount = gardens.Count,
                PlantsCount = plants.Count,
                ActivePlantsCount = plants.Count(p => p.Status == PlantStatus.Active),
                NotesCount = plants.Sum(p => p.Notes.Count),
                CompletedTasksCount = plants.Sum(p => p.Treatments.Count(t =>
                    t.Notes != "Zaplanowane" &&
                    t.Notes != "Pominięte automatyczne")),
                PlannedTasksCount = plants.Sum(p => p.Treatments.Count(t => t.Notes == "Zaplanowane")),
                WateringsCount = plants.Sum(p => p.Treatments.Count(t =>
                    t.TreatmentType == "Watering" &&
                    t.Notes != "Zaplanowane" &&
                    t.Notes != "Pominięte automatyczne")),
                FertilizingsCount = plants.Sum(p => p.Treatments.Count(t =>
                    t.TreatmentType == "Fertilizing" &&
                    t.Notes != "Zaplanowane" &&
                    t.Notes != "Pominięte automatyczne")),
                SprayingsCount = plants.Sum(p => p.Treatments.Count(t =>
                    t.TreatmentType == "Spraying" &&
                    t.Notes != "Zaplanowane" &&
                    t.Notes != "Pominięte automatyczne")),
                HarvestsCount = plants.Sum(p => p.Harvests.Count),
                TotalHarvest = plants.Sum(p => p.Harvests.Sum(h => h.Quantity)),
                PhotosCount = plants.Sum(p => p.Photos.Count)
            };

            return Ok(stats);
        }
    }
}
