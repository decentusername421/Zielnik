using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;
using System.Security.Claims;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlantCategoriesController : ControllerBase
    {
        private readonly ZielnikDbContext _context;

        public PlantCategoriesController(ZielnikDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.PlantCategories
                .Where(c => c.IsApproved)
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToListAsync();

            return Ok(categories);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PlantCategory>> GetCategory(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var category = await _context.PlantCategories
                .FirstOrDefaultAsync(c => c.Id == id &&
                    (c.IsApproved || c.CreatedByUserId == userId || User.IsInRole("Admin")));

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }


        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult> CreateCategory(CreateCategoryDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var name = dto.Name.Trim();

            if (await _context.PlantCategories.AnyAsync(c => c.Name.ToLower() == name.ToLower()))
                return Conflict("Kategoria o tej nazwie już istnieje lub oczekuje na zatwierdzenie.");

            var category = new PlantCategory
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsApproved = isAdmin,
                CreatedByUserId = isAdmin ? null : userId
            };

            _context.PlantCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new
            {
                category.Id,
                category.Name,
                category.IsApproved
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingCategories()
        {
            var categories = await _context.PlantCategories
                .Where(c => !c.IsApproved)
                .Select(c => new { c.Id, c.Name, c.CreatedByUserId })
                .ToListAsync();

            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveCategory(Guid id)
        {
            var category = await _context.PlantCategories.FindAsync(id);

            if (category == null)
                return NotFound();

            category.IsApproved = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectCategory(Guid id)
        {
            var category = await _context.PlantCategories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsApproved);

            if (category == null)
                return NotFound();

            _context.PlantCategories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
public async Task<IActionResult> UpdateCategory(
    Guid id,
    UpdateCategoryDto dto)
{
    var category = await _context.PlantCategories.FindAsync(id);

    if (category == null)
    {
        return NotFound();
    }

    category.Name = dto.Name;

    await _context.SaveChangesAsync();

    return NoContent();
}

        // DELETE: api/categories/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _context.PlantCategories
                .Include(c => c.Plants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _context.PlantCategories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
