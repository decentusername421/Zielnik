using System.ComponentModel.DataAnnotations;

namespace Zielnik.Entities
{
    public class PlantTreatment
    {
        public Guid Id { get; set; }

        public Guid UserPlantId { get; set; }

        public UserPlant UserPlant { get; set; } = null!;

        [Required]
        public string TreatmentType { get; set; } = string.Empty;

        public string? ProductName { get; set; }

        public decimal? Quantity { get; set; }

        public string? Unit { get; set; }

        public string? Notes { get; set; }

        public DateTime PerformedAt { get; set; }
    }
}