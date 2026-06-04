using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;

namespace Zielnik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var category = await _context.PlantCategories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }


        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<PlantCategory>> CreateCategory(PlantCategory category)
        {
            _context.PlantCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.Id },
                category);
        }

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