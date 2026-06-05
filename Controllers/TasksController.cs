using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Zielnik.Data;
using Zielnik.Entities;


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
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var today = DateTime.UtcNow.Date;

            var plants = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .Include(up => up.Treatments)
                .Where(up => up.Garden.UserId == userId)
                .ToListAsync();

            var tasks = new List<object>();

            foreach (var plant in plants)
            {
                if (!plant.PlantingDate.HasValue)
                    continue;

                var days = (today - plant.PlantingDate.Value.Date).Days;

                // Watering
                if (plant.Plant.WateringFrequencyDays > 0 &&
                   days % plant.Plant.WateringFrequencyDays == 0)
                {
                    var alreadyDone = plant.Treatments.Any(t =>
                        t.TreatmentType == "Watering" &&
                        t.PerformedAt.Date == today);

                    if (!alreadyDone)
                    {
                        tasks.Add(new
                        {
                            PlantId = plant.Id,
                            Plant = plant.Plant.Name,
                            Nickname = plant.Nickname,
                            Task = "Watering",
                            DueDate = today
                        });
                    }
                }

                // Fertilizing
                if (plant.Plant.FertilizingFrequencyDays > 0 &&
                    days % plant.Plant.FertilizingFrequencyDays == 0)
                {
                    var alreadyDone = plant.Treatments.Any(t =>
                        t.TreatmentType == "Fertilizing" &&
                        t.PerformedAt.Date == today);

                    if (!alreadyDone)
                    {
                        tasks.Add(new
                        {
                            PlantId = plant.Id,
                            Plant = plant.Plant.Name,
                            Nickname = plant.Nickname,
                            Task = "Fertilizing",
                            DueDate = today
                        });
                    }
                }

                // Spraying
                if (plant.Plant.SprayingFrequencyDays > 0 &&
                    days % plant.Plant.SprayingFrequencyDays == 0)
                {
                    var alreadyDone = plant.Treatments.Any(t =>
                        t.TreatmentType == "Spraying" &&
                        t.PerformedAt.Date == today);

                    if (!alreadyDone)
                    {
                        tasks.Add(new
                        {
                            PlantId = plant.Id,
                            Plant = plant.Plant.Name,
                            Nickname = plant.Nickname,
                            Task = "Spraying",
                            DueDate = today
                        });
                    }
                }

                // Harvest
                if (plant.NextHarvestReminder.HasValue &&
                plant.NextHarvestReminder.Value.Date <= today)
                {
                    tasks.Add(new
                    {
                        PlantId = plant.Id,
                        Plant = plant.Plant.Name,
                        Nickname = plant.Nickname,
                        Task = "Harvest",
                        DueDate = plant.NextHarvestReminder.Value.Date
                    });
                }
            }

            return Ok(tasks);

        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);
            var today = DateTime.UtcNow.Date;

            var plants = await _context.UserPlants
    .Include(up => up.Plant)
    .Include(up => up.Garden)
    .Where(up => up.Garden.UserId == userId)
    .ToListAsync();

            var notifications = plants
                .Where(up =>
                    up.PlantingDate.HasValue &&
                    up.Plant.WateringFrequencyDays > 0 &&
                    (today - up.PlantingDate.Value.Date).Days %
                    up.Plant.WateringFrequencyDays == 0)
                .Select(up =>
                    $"Podlej roślinę: {up.Plant.Name} ({up.Nickname})");

            return Ok(notifications);
        }

        [HttpGet("plant/{userPlantId}")]
        public async Task<IActionResult> GetPlantTasks(Guid userPlantId)
        {
            var userId = User.FindFirstValue(
    ClaimTypes.NameIdentifier);

            var userPlant = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up =>
                    up.Id == userPlantId &&
                    up.Garden.UserId == userId);

            if (userPlant == null)
                return NotFound("Plant not found");

            if (!userPlant.PlantingDate.HasValue)
                return BadRequest("Planting date is missing");

            if (userPlant.Plant.WateringFrequencyDays <= 0)
                return BadRequest("Invalid watering frequency");

            var tasks = new List<object>();

            var nextDate = userPlant.PlantingDate.Value.Date;
            var today = DateTime.UtcNow.Date;

            while (nextDate <= today.AddDays(30))
            {
                tasks.Add(new
                {
                    Task = "Watering",
                    DueDate = nextDate
                });

                nextDate = nextDate.AddDays(
                    userPlant.Plant.WateringFrequencyDays);
            }

            return Ok(tasks);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingTasks(
            [FromQuery] int days = 7)
        {
            var userId = User.FindFirstValue(
    ClaimTypes.NameIdentifier);

            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(days);

            var plants = await _context.UserPlants
    .Include(up => up.Plant)
    .Include(up => up.Garden)
    .Where(up => up.Garden.UserId == userId)
    .ToListAsync();

            var tasks = new List<object>();

            foreach (var plant in plants)
            {
                if (!plant.PlantingDate.HasValue)
                    continue;

                if (plant.Plant.WateringFrequencyDays <= 0)
                    continue;

                var current = plant.PlantingDate.Value.Date;

                while (current <= endDate)
                {
                    if (current >= today)
                    {
                        tasks.Add(new
                        {
                            PlantId = plant.Id,
                            PlantName = plant.Plant.Name,
                            Nickname = plant.Nickname,
                            Task = "Watering",
                            DueDate = current
                        });
                    }

                    current = current.AddDays(
                        plant.Plant.WateringFrequencyDays);
                }
            }

            return Ok(tasks.OrderBy(t =>
                ((DateTime)t.GetType()
                    .GetProperty("DueDate")!
                    .GetValue(t)!)));
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteTask(
    CompleteTaskDto dto)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var plant = await _context.UserPlants
                .Include(x => x.Garden)
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.UserPlantId &&
                    x.Garden.UserId == userId);

            if (plant == null)
                return NotFound();

            if (dto.TaskType == "Harvest")
            {
                return BadRequest(
                    "Harvest should be created via HarvestsController");
            }

            var treatment = new PlantTreatment
            {
                UserPlantId = dto.UserPlantId,
                TreatmentType = dto.TaskType,
                Notes = dto.Notes,
                PerformedAt = DateTime.UtcNow
            };

            _context.PlantTreatments.Add(treatment);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}