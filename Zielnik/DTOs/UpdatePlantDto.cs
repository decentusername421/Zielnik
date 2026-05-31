using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class UpdatePlantDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Species { get; set; } = string.Empty;

        [Range(1, 365)]
        public int WateringFrequencyDays { get; set; }
    }
}