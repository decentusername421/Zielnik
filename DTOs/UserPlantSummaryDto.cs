namespace Zielnik.DTOs
{
    public class UserPlantSummaryDto
    {
        public int NotesCount { get; set; }

        public int TreatmentsCount { get; set; }

        public int HarvestsCount { get; set; }

        public int PhotosCount { get; set; }

        public decimal TotalHarvest { get; set; }

        public DateTime? LastHarvestDate { get; set; }
    }
}