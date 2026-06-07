namespace Zielnik.DTOs
{
    public class UpdateHarvestDto
    {
        public DateTime HarvestDate { get; set; }

        public decimal Quantity { get; set; }

        public string? Unit { get; set; }

        public int FruitsCount { get; set; }

        public string? Notes { get; set; }
    }
}