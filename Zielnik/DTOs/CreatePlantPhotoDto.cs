using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class CreatePlantPhotoDto
    {
        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}