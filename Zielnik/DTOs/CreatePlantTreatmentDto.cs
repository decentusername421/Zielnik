using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class CreatePlantTreatmentDto
    {
        [Required]
        public string TreatmentType { get; set; } = string.Empty;

        public string? ProductName { get; set; }

        public decimal? Quantity { get; set; }

        public string? Unit { get; set; }

        public string? Notes { get; set; }

        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}