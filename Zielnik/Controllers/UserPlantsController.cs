using Microsoft.AspNetCore.Mvc;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserPlant(Guid id)
        {
            var userPlant = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .FirstOrDefaultAsync(up => up.Id == id);

            if (userPlant == null)
                return NotFound();

            return Ok(new
            {
                userPlant.Id,
                userPlant.Nickname,
                userPlant.Status,
                PlantName = userPlant.Plant.Name,
                GardenName = userPlant.Garden.Name,
                userPlant.SowingDate,
                userPlant.PlantingDate
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPlants()
        {
            var userPlants = await _context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.Garden)
                .Select(up => new
                {
                    up.Id,
                    up.Nickname,
                    up.Status,

                    PlantName = up.Plant.Name,
                    GardenName = up.Garden.Name,

                    up.SowingDate,
                    up.PlantingDate
                })
                .ToListAsync();

            return Ok(userPlants);
        }

        [HttpGet("{id}/notes")]
        public async Task<IActionResult> GetNotes(Guid id)
        {
            var notes = await _context.PlantNotes
                .Where(n => n.UserPlantId == id)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content,
                    n.CreatedAt
                })
                .ToListAsync();

            return Ok(notes);
        }

        [HttpGet("{id}/treatments")]
        public async Task<IActionResult> GetTreatments(Guid id)
        {
            var treatments = await _context.PlantTreatments
                .Where(t => t.UserPlantId == id)
                .Select(t => new
                {
                    t.Id,
                    t.TreatmentType,
                    t.ProductName,
                    t.Quantity,
                    t.Unit,
                    t.Notes,
                    t.PerformedAt
                })
                .ToListAsync();

            return Ok(treatments);
        }

        [HttpGet("{id}/harvests")]
        public async Task<IActionResult> GetHarvests(Guid id)
        {
            var harvests = await _context.Harvests
                .Where(h => h.UserPlantId == id)
                .Select(h => new
                {
                    h.Id,
                    h.HarvestDate,
                    h.Quantity,
                    h.Unit,
                    h.FruitsCount,
                    h.Notes
                })
                .ToListAsync();

            return Ok(harvests);
        }

        [HttpGet("{id}/photos")]
        public async Task<IActionResult> GetPhotos(Guid id)
        {
            var photos = await _context.PlantPhotos
                .Where(p => p.UserPlantId == id)
                .Select(p => new
                {
                    p.Id,
                    p.FilePath,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(photos);
        }

        [HttpPost("{id}/notes")]
        public async Task<IActionResult> CreateNote(
    Guid id,
    CreatePlantNoteDto dto)
        {
            var userPlant = await _context.UserPlants.FindAsync(id);

            if (userPlant == null)
                return NotFound("UserPlant not found.");

            var note = new PlantNote
            {
                Id = Guid.NewGuid(),
                UserPlantId = id,
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.PlantNotes.Add(note);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                note.Id,
                note.UserPlantId,
                note.Title,
                note.Content,
                note.CreatedAt
            });
        }

        [HttpDelete("notes/{noteId}")]
        public async Task<IActionResult> DeleteNote(Guid noteId)
        {
            var note = await _context.PlantNotes.FindAsync(noteId);

            if (note == null)
                return NotFound();

            _context.PlantNotes.Remove(note);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/treatments")]
        public async Task<IActionResult> CreateTreatment(
    Guid id,
    CreatePlantTreatmentDto dto)
        {
            var userPlant = await _context.UserPlants.FindAsync(id);

            if (userPlant == null)
                return NotFound();

            var treatment = new PlantTreatment
            {
                Id = Guid.NewGuid(),
                UserPlantId = id,
                TreatmentType = dto.TreatmentType,
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                Unit = dto.Unit,
                Notes = dto.Notes,
                PerformedAt = dto.PerformedAt
            };

            _context.PlantTreatments.Add(treatment);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                treatment.Id,
                treatment.UserPlantId,
                treatment.TreatmentType,
                treatment.ProductName,
                treatment.Quantity,
                treatment.Unit,
                treatment.Notes,
                treatment.PerformedAt
            });
        }

        [HttpPost("{id}/harvests")]
        public async Task<IActionResult> CreateHarvest(
    Guid id,
    CreateHarvestDto dto)
        {
            var userPlant = await _context.UserPlants.FindAsync(id);

            if (userPlant == null)
                return NotFound();

            var harvest = new Harvest
            {
                Id = Guid.NewGuid(),
                UserPlantId = id,
                HarvestDate = dto.HarvestDate,
                Quantity = dto.Quantity,
                Unit = dto.Unit,
                FruitsCount = dto.FruitsCount,
                Notes = dto.Notes
            };

            _context.Harvests.Add(harvest);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                harvest.Id,
                harvest.UserPlantId,
                harvest.HarvestDate,
                harvest.Quantity,
                harvest.Unit,
                harvest.FruitsCount,
                harvest.Notes
            });
        }

        [HttpPost("{id}/photos")]
        public async Task<IActionResult> CreatePhoto(
     Guid id,
     [FromForm] CreatePlantPhotoDto dto)
        {
            var userPlant = await _context.UserPlants.FindAsync(id);

            if (userPlant == null)
                return NotFound();

            if (dto.Photo == null || dto.Photo.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "photos");

            Directory.CreateDirectory(uploadsFolder);

            var fileName =
                $"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Photo.CopyToAsync(stream);
            }

            var photo = new PlantPhoto
            {
                Id = Guid.NewGuid(),
                UserPlantId = id,
                FilePath = $"/uploads/photos/{fileName}",
                CreatedAt = DateTime.UtcNow
            };

            _context.PlantPhotos.Add(photo);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                photo.Id,
                photo.UserPlantId,
                photo.FilePath,
                photo.CreatedAt
            });
        }

        [HttpDelete("treatments/{treatmentId}")]
        public async Task<IActionResult> DeleteTreatment(Guid treatmentId)
        {
            var treatment = await _context.PlantTreatments.FindAsync(treatmentId);

            if (treatment == null)
                return NotFound();

            _context.PlantTreatments.Remove(treatment);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("photos/{photoId}")]
        public async Task<IActionResult> DeletePhoto(Guid photoId)
        {
            var photo = await _context.PlantPhotos.FindAsync(photoId);

            if (photo == null)
                return NotFound();

            // Ścieżka fizyczna pliku
            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                photo.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            // Usuń plik z dysku jeśli istnieje
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            // Usuń rekord z bazy
            _context.PlantPhotos.Remove(photo);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("harvests/{harvestId}")]
        public async Task<IActionResult> DeleteHarvest(Guid harvestId)
        {
            var harvest = await _context.Harvests.FindAsync(harvestId);

            if (harvest == null)
                return NotFound();

            _context.Harvests.Remove(harvest);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("notes/{noteId}")]
        public async Task<IActionResult> UpdateNote(
    Guid noteId,
    UpdatePlantNoteDto dto)
        {
            var note = await _context.PlantNotes.FindAsync(noteId);

            if (note == null)
                return NotFound();

            note.Title = dto.Title;
            note.Content = dto.Content;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                note.Id,
                note.Title,
                note.Content,
                note.CreatedAt
            });
        }

        [HttpPut("treatments/{treatmentId}")]
        public async Task<IActionResult> UpdateTreatment(
    Guid treatmentId,
    UpdatePlantTreatmentDto dto)
        {
            var treatment = await _context.PlantTreatments.FindAsync(treatmentId);

            if (treatment == null)
                return NotFound();

            treatment.TreatmentType = dto.TreatmentType;
            treatment.ProductName = dto.ProductName;
            treatment.Quantity = dto.Quantity;
            treatment.Unit = dto.Unit;
            treatment.Notes = dto.Notes;
            treatment.PerformedAt = dto.PerformedAt;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                treatment.Id,
                treatment.TreatmentType,
                treatment.ProductName,
                treatment.Quantity,
                treatment.Unit,
                treatment.Notes,
                treatment.PerformedAt
            });
        }

        [HttpPut("harvests/{harvestId}")]
        public async Task<IActionResult> UpdateHarvest(
    Guid harvestId,
    UpdateHarvestDto dto)
        {
            var harvest = await _context.Harvests.FindAsync(harvestId);

            if (harvest == null)
                return NotFound();

            harvest.HarvestDate = dto.HarvestDate;
            harvest.Quantity = dto.Quantity;
            harvest.Unit = dto.Unit;
            harvest.FruitsCount = dto.FruitsCount;
            harvest.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                harvest.Id,
                harvest.HarvestDate,
                harvest.Quantity,
                harvest.Unit,
                harvest.FruitsCount,
                harvest.Notes
            });
        }

        [HttpGet("{id}/summary")]
        public async Task<ActionResult<UserPlantSummaryDto>> GetSummary(Guid id)
        {
            var userPlantExists = await _context.UserPlants
                .AnyAsync(up => up.Id == id);

            if (!userPlantExists)
            {
                return NotFound("UserPlant not found.");
            }

            var summary = new UserPlantSummaryDto
            {
                NotesCount = await _context.PlantNotes
                    .CountAsync(n => n.UserPlantId == id),

                TreatmentsCount = await _context.PlantTreatments
                    .CountAsync(t => t.UserPlantId == id),

                HarvestsCount = await _context.Harvests
                    .CountAsync(h => h.UserPlantId == id),

                PhotosCount = await _context.PlantPhotos
                    .CountAsync(p => p.UserPlantId == id),

                TotalHarvest = await _context.Harvests
                    .Where(h => h.UserPlantId == id)
                    .SumAsync(h => (decimal?)h.Quantity) ?? 0,

                LastHarvestDate = await _context.Harvests
                    .Where(h => h.UserPlantId == id)
                    .MaxAsync(h => (DateTime?)h.HarvestDate)
            };

            return Ok(summary);
        }
    }
}