using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class CreatePlantNoteDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Content { get; set; }
    }
}