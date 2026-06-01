using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
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
        public async Task<ActionResult<IEnumerable<PlantCategory>>> GetCategories()
        {
            return await _context.PlantCategories.ToListAsync();
        }

        // GET: api/categories/5
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
        public async Task<IActionResult> UpdateCategory(Guid id, PlantCategory? category)
        {
            if (category == null)
            {
                return BadRequest();
            }

            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PlantCategories.Any(c => c.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _context.PlantCategories.FindAsync(id);

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