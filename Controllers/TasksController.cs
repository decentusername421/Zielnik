using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;

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
            var today = DateTime.UtcNow.Date;

            var plants = await _context.UserPlants
                .Include(up => up.Plant)
                .ToListAsync();

            var tasks = plants
                .Where(up =>
                    up.PlantingDate.HasValue &&
                    up.Plant.WateringFrequencyDays > 0 &&
                    ((today - up.PlantingDate.Value.Date).Days %
                     up.Plant.WateringFrequencyDays == 0))
                .Select(up => new
                {
                    Plant = up.Plant.Name,
                    Nickname = up.Nickname,
                    Task = "Watering",
                    Today = today
                });

            return Ok(tasks);
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var today = DateTime.UtcNow.Date;

            var plants = await _context.UserPlants
                .Include(up => up.Plant)
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
            var userPlant = await _context.UserPlants
                .Include(up => up.Plant)
                .FirstOrDefaultAsync(up => up.Id == userPlantId);

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
            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(days);

            var plants = await _context.UserPlants
                .Include(up => up.Plant)
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
    }
}