using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.Entities;
using System.Security.Claims;
using Zielnik.DTOs;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public TasksController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodayTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var today = DateTime.UtcNow.Date;

            var plants = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .Include(up => up.Treatments)
                .Where(up => up.Garden.UserId == userId)
                .ToListAsync();

            var tasks = new List<object>();

            var plannedTasks = await _context.PlantTreatments
                .Include(t => t.UserPlant)
                .ThenInclude(up => up.Plant)
                .Include(t => t.UserPlant)
                .ThenInclude(up => up.Garden)
                .Where(t => t.UserPlant.Garden.UserId == userId &&
                            t.Notes == "Zaplanowane" &&
                            t.PerformedAt.Date == today)
                .Select(t => new
                {
                    userPlantId = t.UserPlantId,
                    plantName = t.UserPlant.Plant.Name,
                    taskType = t.TreatmentType,
                    dueDate = t.PerformedAt.Date
                })
                .ToListAsync();

            tasks.AddRange(plannedTasks);

            foreach (var plant in plants)
            {
                if (!plant.PlantingDate.HasValue) continue;
                var days = (today - plant.PlantingDate.Value.Date).Days;

                if (plant.Plant.WateringFrequencyDays > 0 && days % plant.Plant.WateringFrequencyDays == 0)
                {
                    var alreadyDone = plant.Treatments.Any(t => t.TreatmentType == "Watering" && t.Notes != "Zaplanowane" && t.PerformedAt.Date == today);
                    if (!alreadyDone) tasks.Add(new { userPlantId = plant.Id, plantName = plant.Plant.Name, taskType = "Watering", dueDate = today });
                }
                if (plant.Plant.FertilizingFrequencyDays > 0 && days % plant.Plant.FertilizingFrequencyDays == 0)
                {
                    var alreadyDone = plant.Treatments.Any(t => t.TreatmentType == "Fertilizing" && t.Notes != "Zaplanowane" && t.PerformedAt.Date == today);
                    if (!alreadyDone) tasks.Add(new { userPlantId = plant.Id, plantName = plant.Plant.Name, taskType = "Fertilizing", dueDate = today });
                }
                if (plant.Plant.SprayingFrequencyDays > 0 && days % plant.Plant.SprayingFrequencyDays == 0)
                {
                    var alreadyDone = plant.Treatments.Any(t => t.TreatmentType == "Spraying" && t.Notes != "Zaplanowane" && t.PerformedAt.Date == today);
                    if (!alreadyDone) tasks.Add(new { userPlantId = plant.Id, plantName = plant.Plant.Name, taskType = "Spraying", dueDate = today });
                }
                if (plant.Plant.HarvestAfterDays > 0 && days >= plant.Plant.HarvestAfterDays)
                {
                    var alreadyDone = plant.Treatments.Any(t => t.TreatmentType == "Harvest" && t.Notes != "Zaplanowane" && t.PerformedAt.Date == today);
                    if (!alreadyDone) tasks.Add(new { userPlantId = plant.Id, plantName = plant.Plant.Name, taskType = "Harvest", dueDate = today });
                }
            }
            return Ok(tasks);
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var today = DateTime.UtcNow.Date;
            var plants = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .Where(up => up.Garden.UserId == userId)
                .ToListAsync();

            var notifications = plants
                .Where(up => up.PlantingDate.HasValue && up.Plant.WateringFrequencyDays > 0 && (today - up.PlantingDate.Value.Date).Days % up.Plant.WateringFrequencyDays == 0)
                .Select(up => $"Podlej roślinę: {up.Plant.Name} ({up.Nickname})");

            return Ok(notifications);
        }

        [HttpGet("plant/{userPlantId}")]
        public async Task<IActionResult> GetPlantTasks(Guid userPlantId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userPlant = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == userPlantId && up.Garden.UserId == userId);

            if (userPlant == null) return NotFound("Plant not found");
            if (!userPlant.PlantingDate.HasValue) return BadRequest("Planting date is missing");

            var tasks = new List<object>();
            var nextDate = userPlant.PlantingDate.Value.Date;
            var today = DateTime.UtcNow.Date;

            while (nextDate <= today.AddDays(30))
            {
                tasks.Add(new { Task = "Watering", DueDate = nextDate });
                nextDate = nextDate.AddDays(userPlant.Plant.WateringFrequencyDays);
            }
            return Ok(tasks);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingTasks([FromQuery] int days = 7)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(days);

            var plants = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .Where(up => up.Garden.UserId == userId)
                .ToListAsync();

            var tasks = new List<object>();

            var plannedTasks = await _context.PlantTreatments
                .Include(t => t.UserPlant)
                .ThenInclude(up => up.Plant)
                .Include(t => t.UserPlant)
                .ThenInclude(up => up.Garden)
                .Where(t => t.UserPlant.Garden.UserId == userId &&
                            t.Notes == "Zaplanowane" &&
                            t.PerformedAt.Date >= today &&
                            t.PerformedAt.Date <= endDate)
                .Select(t => new
                {
                    PlantId = t.UserPlantId,
                    PlantName = t.UserPlant.Plant.Name,
                    t.UserPlant.Nickname,
                    Task = t.TreatmentType,
                    DueDate = t.PerformedAt.Date
                })
                .ToListAsync();

            tasks.AddRange(plannedTasks);

            foreach (var plant in plants)
            {
                if (!plant.PlantingDate.HasValue || plant.Plant.WateringFrequencyDays <= 0) continue;
                var current = plant.PlantingDate.Value.Date;
                while (current <= endDate)
                {
                    if (current >= today)
                        tasks.Add(new { PlantId = plant.Id, PlantName = plant.Plant.Name, Nickname = plant.Nickname, Task = "Watering", DueDate = current });
                    current = current.AddDays(plant.Plant.WateringFrequencyDays);
                }
            }
            return Ok(tasks.OrderBy(t => ((DateTime)t.GetType().GetProperty("DueDate")!.GetValue(t)!)));
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteTask([FromBody] TaskCompletionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userPlant = await _context.UserPlants
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == dto.UserPlantId && up.Garden.UserId == userId);

            if (userPlant == null) return NotFound("Plant not found");

            var today = DateTime.UtcNow.Date;
            var planned = await _context.PlantTreatments
                .FirstOrDefaultAsync(t => t.UserPlantId == dto.UserPlantId &&
                                          t.TreatmentType == dto.TaskType &&
                                          t.Notes == "Zaplanowane" &&
                                          t.PerformedAt.Date == today);

            if (planned != null)
            {
                planned.Notes = null;
                planned.PerformedAt = DateTime.UtcNow;
            }
            else
            {
                var treatment = new PlantTreatment
                {
                    Id = Guid.NewGuid(),
                    UserPlantId = dto.UserPlantId,
                    TreatmentType = dto.TaskType,
                    PerformedAt = DateTime.UtcNow
                };

                _context.PlantTreatments.Add(treatment);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("add-manual")]
        public async Task<IActionResult> AddManualTask([FromBody] ManualTaskDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userPlant = await _context.UserPlants
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == dto.UserPlantId && up.Garden.UserId == userId);

            if (userPlant == null) return NotFound("Plant not found");

            var treatment = new PlantTreatment
            {
                Id = Guid.NewGuid(),
                UserPlantId = dto.UserPlantId,
                TreatmentType = dto.TaskType,
                PerformedAt = dto.DueDate.Date,
                Notes = "Zaplanowane"
            };

            _context.PlantTreatments.Add(treatment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTaskHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _context.PlantTreatments
                .Include(t => t.UserPlant)
                .ThenInclude(up => up.Plant)
                .Where(t => t.UserPlant.Garden.UserId == userId && t.Notes != "Zaplanowane")
                .OrderByDescending(t => t.PerformedAt)
                .Take(10)
                .Select(t => new {
                    plantName = t.UserPlant.Plant.Name,
                    type = t.TreatmentType,
                    date = t.PerformedAt
                })
                .ToListAsync();

            return Ok(history);
        }
    }
}
