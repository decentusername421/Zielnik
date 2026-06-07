namespace Zielnik.DTOs
{
    public class CreateHarvestDto
    {
        public Guid UserPlantId { get; set; }
        public DateTime HarvestDate { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; } = "kg";

        public int? FruitsCount { get; set; }

        public string? Notes { get; set; }
    }
}