using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class GardensController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public GardensController(ZielnikDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetGardens()
        {
            var gardens = await _context.Gardens.ToListAsync();
            return Ok(gardens);
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> CreateGarden(CreateGardenDto dto)
        {
            var garden = new Entities.Garden
            {
                Name = dto.Name
            };

            _context.Gardens.Add(garden);
            await _context.SaveChangesAsync();

            return Ok(garden);
        }


    }
}