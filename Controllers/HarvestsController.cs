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
    public class HarvestsController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public HarvestsController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHarvest(
     CreateHarvestDto dto)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var plant = await _context.UserPlants
                .Include(x => x.Garden)
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.UserPlantId &&
                    x.Garden.UserId == userId);

            if (plant == null)
                return NotFound("Plant not found");

            var harvest = new Harvest
            {
                UserPlantId = dto.UserPlantId,
                HarvestDate = dto.HarvestDate,
                Quantity = dto.Quantity,
                Unit = dto.Unit,
                FruitsCount = dto.FruitsCount,
                Notes = dto.Notes
            };

            _context.Harvests.Add(harvest);

            // ustaw kolejne przypomnienie
            if (plant.HarvestReminderDays.HasValue)
            {
                plant.NextHarvestReminder =
                    dto.HarvestDate.Date.AddDays(
                        plant.HarvestReminderDays.Value);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetHarvest),
                new { id = harvest.Id },
                harvest);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetHarvest(Guid id)
        {
            var harvest = await _context.Harvests
                .Include(h => h.UserPlant)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (harvest == null)
                return NotFound();

            return Ok(harvest);
        }

        [HttpGet("plant/{userPlantId}")]
        public async Task<IActionResult> GetPlantHarvests(
    Guid userPlantId)
        {
            var harvests = await _context.Harvests
                .Where(h => h.UserPlantId == userPlantId)
                .OrderByDescending(h => h.HarvestDate)
                .ToListAsync();

            return Ok(harvests);
        }
    }
}