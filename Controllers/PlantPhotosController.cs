using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zielnik.Data;
using Zielnik.DTOs;
using Zielnik.Entities;

namespace Zielnik.Controllers;

[Authorize]
[ApiController]
[Route("api/userplants/{userPlantId}/photos")]
public class PlantPhotosController : ControllerBase
{
    private readonly ZielnikDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public PlantPhotosController(
        ZielnikDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> GetPhotos(Guid userPlantId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var photos = await _context.PlantPhotos
            .Include(p => p.UserPlant)
                .ThenInclude(up => up.Garden)
            .Where(p =>
                p.UserPlantId == userPlantId &&
                p.UserPlant.Garden.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.FilePath,
                p.CreatedAt
            })
            .ToListAsync();

        return Ok(photos);
    }

    //[HttpGet("{photoId}")]
    //public async Task<IActionResult> GetPhoto(
    //    Guid userPlantId,
    //    Guid photoId)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //    var photo = await _context.PlantPhotos
    //        .Include(p => p.UserPlant)
    //            .ThenInclude(up => up.Garden)
    //        .FirstOrDefaultAsync(p =>
    //            p.Id == photoId &&
    //            p.UserPlantId == userPlantId &&
    //            p.UserPlant.Garden.UserId == userId);

    //    if (photo == null)
    //        return NotFound();

    //    return Ok(photo);
    //}

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPhoto(
        Guid userPlantId,
        [FromForm] CreatePlantPhotoDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userPlant = await _context.UserPlants
            .Include(up => up.Garden)
            .FirstOrDefaultAsync(up =>
                up.Id == userPlantId &&
                up.Garden.UserId == userId);

        if (userPlant == null)
            return NotFound("Nie znaleziono rośliny.");

        if (dto.Photo == null || dto.Photo.Length == 0)
            return BadRequest("Nie wybrano pliku.");

        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads");

        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(dto.Photo.FileName);

        var fileName = $"{Guid.NewGuid()}{extension}";

        var physicalPath = Path.Combine(
            uploadsFolder,
            fileName);

        using (var stream = new FileStream(
            physicalPath,
            FileMode.Create))
        {
            await dto.Photo.CopyToAsync(stream);
        }

        var plantPhoto = new PlantPhoto
        {
            Id = Guid.NewGuid(),
            UserPlantId = userPlantId,
            FilePath = $"/uploads/{fileName}",
            CreatedAt = DateTime.UtcNow
        };

        _context.PlantPhotos.Add(plantPhoto);

        await _context.SaveChangesAsync();

        return Created(
    plantPhoto.FilePath,
    plantPhoto);
    }

    [HttpDelete("{photoId}")]
    public async Task<IActionResult> DeletePhoto(
        Guid userPlantId,
        Guid photoId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var photo = await _context.PlantPhotos
     .Include(p => p.UserPlant)
     .ThenInclude(up => up.Garden)
     .FirstOrDefaultAsync(p =>
         p.Id == photoId &&
         p.UserPlant.Garden.UserId == userId);

        if (photo == null)
            return NotFound();

        var physicalPath = Path.Combine(
            _environment.WebRootPath,
            photo.FilePath.TrimStart('/'));

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        _context.PlantPhotos.Remove(photo);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}