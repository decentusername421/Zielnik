namespace Zielnik.DTOs
{
    public class PlantStatisticsDto
    {
        public Guid UserPlantId { get; set; }

        public string PlantName { get; set; } = string.Empty;

        public string? Nickname { get; set; }

        public int NotesCount { get; set; }

        public int TreatmentsCount { get; set; }

        public int WateringsCount { get; set; }

        public int FertilizingsCount { get; set; }

        public int SprayingsCount { get; set; }

        public int HarvestsCount { get; set; }

        public decimal TotalHarvest { get; set; }

        public int TotalFruits { get; set; }

        public int PhotosCount { get; set; }
    }
}