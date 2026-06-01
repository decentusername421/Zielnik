using Microsoft.AspNetCore.Mvc;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPlantsController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public UserPlantsController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<UserPlant>> CreateUserPlant(
            CreateUserPlantDto dto)
        {
            var userPlant = new UserPlant
            {
                Id = Guid.NewGuid(),
                PlantId = dto.PlantId,
                GardenId = dto.GardenId,
                SowingDate = dto.SowingDate,
                PlantingDate = dto.PlantingDate,
                Nickname = dto.Nickname,
                Status = PlantStatus.Active
            };

            _context.UserPlants.Add(userPlant);
            await _context.SaveChangesAsync();

            return Ok(userPlant);
        }
    }
}