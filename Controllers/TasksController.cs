using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Microsoft.AspNetCore.Authorization;

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
                     up.Plant.WateringFrequencyDays == 0)
                )
                .Select(up => new
                {
                    Plant = up.Plant.Name,
                    Nickname = up.Nickname,
                    Task = "Podlewanie",
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
                    (today - up.PlantingDate.Value.Date).Days %
                    up.Plant.WateringFrequencyDays == 0)
                .Select(up => $"Podlej roślinę: {up.Plant.Name} ({up.Nickname})");

            return Ok(notifications);
        }
    }
}