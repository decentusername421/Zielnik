using System.ComponentModel.DataAnnotations;

namespace Zielnik.Entities
{
    public class Plant
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Species { get; set; } = string.Empty;

        [Range(1, 365)]
        public int WateringFrequencyDays { get; set; }

        public List<PlantCategory> Categories { get; set; } = new();
        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsCustomPlant { get; set; }
        public List<UserPlant> UserPlants { get; set; } = new();
    }
}