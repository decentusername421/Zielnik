using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class UpdatePlantNoteDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}