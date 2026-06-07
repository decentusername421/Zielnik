using System.ComponentModel.DataAnnotations;

namespace Zielnik.Entities
{
    public class PlantNote
    {
        public Guid Id { get; set; }

        public Guid UserPlantId { get; set; }

        public UserPlant UserPlant { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}