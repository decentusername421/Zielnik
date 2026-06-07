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
                    id = t.Id,
                    userPlantId = t.UserPlantId,
                    plantName = t.UserPlant.Plant.Name,
                    taskType = t.TreatmentType,
                    dueDate = t.PerformedAt.Date,
                    isManual = true
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
                    if (!alreadyDone) tasks.Add(new { id = (Guid?)null, userPlantId = plant.Id, plantName = plant.Plant.Name, taskType = "Watering", dueDate = today, isManual = false });
                }
                if (plant.Plant.FertilizingFrequencyDays > 0 && days % plant.Plant.FertilizingFrequencyDays == 0)
                {
                    var alreadyDone = plant.Treatments.Any(t => t.TreatmentType == "Fertilizing" && t.Notes != "Zaplanowane" && t.PerformedAt.Date == today);
                    if (!alreadyDone) tasks.Add(new { id = (Guid?)null, userPlantId = plant.Id, plantName = plant.Plant.Name, taskType = "Fertilizing", dueDate = today, isManual = false });
                }
                if (plant.Plant.SprayingFrequencyDays > 0 && days % plant.Plant.SprayingFrequencyDays == 0)
                {
                    var alreadyDone = plant.Treatments.Any(t => t.TreatmentType == "Spraying" && t.Notes != "Zaplanowane" && t.PerformedAt.Date == today);
                    if (!alreadyDone) tasks.Add(new { id = (Guid?)null, userPlantId = plant.Id, plantName = plant.Plant.Name, taskType = "Spraying", dueDate = today, isManual = false });
                }
                if (plant.Plant.HarvestAfterDays > 0 && days >= plant.Plant.HarvestAfterDays)
                {
                    var alreadyDone = plant.Treatments.Any(t => t.TreatmentType == "Harvest" && t.Notes != "Zaplanowane" && t.PerformedAt.Date == today);
                    if (!alreadyDone) tasks.Add(new { id = (Guid?)null, userPlantId = plant.Id, plantName = plant.Plant.Name, taskType = "Harvest", dueDate = today, isManual = false });
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
                .Include(up => up.Treatments)
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
                            t.PerformedAt.Date >= today &&
                            t.PerformedAt.Date <= endDate)
                .Select(t => new
                {
                    Id = t.Id,
                    PlantId = t.UserPlantId,
                    PlantName = t.UserPlant.Plant.Name,
                    t.UserPlant.Nickname,
                    Task = t.TreatmentType,
                    DueDate = t.PerformedAt.Date,
                    IsManual = true
                })
                .ToListAsync();

            tasks.AddRange(plannedTasks);

            foreach (var plant in plants)
            {
                if (!plant.PlantingDate.HasValue) continue;

                AddRecurringTasks(
                    tasks, plant, "Watering",
                    plant.Plant.WateringFrequencyDays, today, endDate);
                AddRecurringTasks(
                    tasks, plant, "Fertilizing",
                    plant.Plant.FertilizingFrequencyDays ?? 0, today, endDate);
                AddRecurringTasks(
                    tasks, plant, "Spraying",
                    plant.Plant.SprayingFrequencyDays ?? 0, today, endDate);

                if (plant.Plant.HarvestAfterDays > 0)
                {
                    var harvestDate = plant.PlantingDate.Value.Date
                        .AddDays(plant.Plant.HarvestAfterDays.Value);
                    var isSkipped = plant.Treatments.Any(t =>
                        t.TreatmentType == "Harvest" &&
                        t.Notes == "Pominięte automatyczne" &&
                        t.PerformedAt.Date == harvestDate);

                    if (harvestDate >= today && harvestDate <= endDate && !isSkipped)
                    {
                        tasks.Add(new
                        {
                            Id = (Guid?)null,
                            PlantId = plant.Id,
                            PlantName = plant.Plant.Name,
                            Nickname = plant.Nickname,
                            Task = "Harvest",
                            DueDate = harvestDate,
                            IsManual = false
                        });
                    }
                }
            }
            return Ok(tasks
                .OrderBy(t => ((DateTime)t.GetType().GetProperty("DueDate")!.GetValue(t)!))
                .ToList());
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteTask([FromBody] TaskCompletionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userPlant = await _context.UserPlants
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == dto.UserPlantId && up.Garden.UserId == userId);

            if (userPlant == null) return NotFound("Plant not found");

            PlantTreatment? planned = null;

            if (dto.TaskId.HasValue)
            {
                planned = await _context.PlantTreatments
                    .FirstOrDefaultAsync(t => t.Id == dto.TaskId &&
                                              t.UserPlantId == dto.UserPlantId &&
                                              t.Notes == "Zaplanowane");
            }

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManualTask(Guid id, [FromBody] UpdateManualTaskDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.PlantTreatments
                .Include(t => t.UserPlant)
                .ThenInclude(up => up.Garden)
                .FirstOrDefaultAsync(t => t.Id == id &&
                                          t.Notes == "Zaplanowane" &&
                                          t.UserPlant.Garden.UserId == userId);

            if (task == null)
                return NotFound();

            var targetPlant = await _context.UserPlants
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == dto.UserPlantId &&
                                           up.Garden.UserId == userId);

            if (targetPlant == null)
                return BadRequest("Nie znaleziono rośliny użytkownika.");

            task.UserPlantId = dto.UserPlantId;
            task.TreatmentType = dto.TaskType;
            task.PerformedAt = dto.DueDate.Date;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManualTask(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Usunąć można wyłącznie własne, ręcznie zaplanowane zadanie.
            var task = await _context.PlantTreatments
                .Include(t => t.UserPlant)
                .ThenInclude(up => up.Garden)
                .FirstOrDefaultAsync(t => t.Id == id &&
                                          t.Notes == "Zaplanowane" &&
                                          t.UserPlant.Garden.UserId == userId);

            if (task == null)
                return NotFound();

            _context.PlantTreatments.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("skip-automatic")]
        public async Task<IActionResult> SkipAutomaticTask([FromBody] AutomaticTaskDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userPlant = await _context.UserPlants
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == dto.UserPlantId &&
                                           up.Garden.UserId == userId);

            if (userPlant == null)
                return NotFound();

            await MarkAutomaticTaskAsSkipped(dto);

            return NoContent();
        }

        [HttpPost("reschedule-automatic")]
        public async Task<IActionResult> RescheduleAutomaticTask(
            [FromBody] RescheduleAutomaticTaskDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var targetPlant = await _context.UserPlants
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == dto.UserPlantId &&
                                           up.Garden.UserId == userId);

            if (targetPlant == null)
                return NotFound();

            await MarkAutomaticTaskAsSkipped(dto);

            _context.PlantTreatments.Add(new PlantTreatment
            {
                Id = Guid.NewGuid(),
                UserPlantId = dto.UserPlantId,
                TreatmentType = dto.TaskType,
                PerformedAt = dto.NewDueDate.Date,
                Notes = "Zaplanowane"
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static void AddRecurringTasks(
            List<object> tasks,
            UserPlant plant,
            string taskType,
            int frequencyDays,
            DateTime today,
            DateTime endDate)
        {
            if (frequencyDays <= 0 || !plant.PlantingDate.HasValue) return;

            var current = plant.PlantingDate.Value.Date;
            while (current < today)
                current = current.AddDays(frequencyDays);

            while (current <= endDate)
            {
                var isSkipped = plant.Treatments.Any(t =>
                    t.TreatmentType == taskType &&
                    t.Notes == "Pominięte automatyczne" &&
                    t.PerformedAt.Date == current);

                if (!isSkipped)
                {
                    tasks.Add(new
                    {
                        Id = (Guid?)null,
                        PlantId = plant.Id,
                        PlantName = plant.Plant.Name,
                        Nickname = plant.Nickname,
                        Task = taskType,
                        DueDate = current,
                        IsManual = false
                    });
                }

                current = current.AddDays(frequencyDays);
            }
        }

        private async Task MarkAutomaticTaskAsSkipped(AutomaticTaskDto dto)
        {
            var alreadySkipped = await _context.PlantTreatments.AnyAsync(t =>
                t.UserPlantId == dto.UserPlantId &&
                t.TreatmentType == dto.TaskType &&
                t.Notes == "Pominięte automatyczne" &&
                t.PerformedAt.Date == dto.DueDate.Date);

            if (alreadySkipped) return;

            _context.PlantTreatments.Add(new PlantTreatment
            {
                Id = Guid.NewGuid(),
                UserPlantId = dto.UserPlantId,
                TreatmentType = dto.TaskType,
                PerformedAt = dto.DueDate.Date,
                Notes = "Pominięte automatyczne"
            });
            await _context.SaveChangesAsync();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTaskHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _context.PlantTreatments
                .Include(t => t.UserPlant)
                .ThenInclude(up => up.Plant)
                .Where(t => t.UserPlant.Garden.UserId == userId &&
                            t.Notes != "Zaplanowane" &&
                            t.Notes != "Pominięte automatyczne")
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
