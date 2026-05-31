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
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var categories = await _context.PlantCategories
    .Include(c => c.Plants)
    .Select(c => new CategoryDto
    {
        Id = c.Id,
        Name = c.Name,
        Plants = c.Plants
            .Select(p => p.Name)
            .ToList()
    })
    .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(Guid id)
        {
            var category = await _context.PlantCategories
    .Include(c => c.Plants)
    .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Plants = category.Plants
        .Select(p => p.Name)
        .ToList()
            });
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
        {
            var category = new PlantCategory
            {
                Name = dto.Name
            };

            _context.PlantCategories.Add(category);

            await _context.SaveChangesAsync();

            return Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Plants = new List<string>()
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(
     Guid id,
     UpdateCategoryDto dto)
        {
            var category = await _context.PlantCategories
                .Include(c => c.Plants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = dto.Name;

            await _context.SaveChangesAsync();

            return Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Plants = category.Plants
                    .Select(p => p.Name)
                    .ToList()
            });
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