using System.ComponentModel.DataAnnotations;

namespace Zielnik.Entities
{
    public class Harvest
    {
        public Guid Id { get; set; }

        public Guid UserPlantId { get; set; }

        public UserPlant UserPlant { get; set; } = null!;

        public DateTime HarvestDate { get; set; }

        public decimal Quantity { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; } = "kg";
        public int? FruitsCount { get; set; }

        public string? Notes { get; set; }
    }
}