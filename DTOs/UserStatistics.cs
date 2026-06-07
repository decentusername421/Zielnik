namespace Zielnik.DTOs
{
    public class UserStatisticsDto
    {
        public int GardensCount { get; set; }

        public int PlantsCount { get; set; }

        public int ActivePlantsCount { get; set; }

        public int HarvestedPlantsCount { get; set; }

        public int DeadPlantsCount { get; set; }

        public int NotesCount { get; set; }

        public int TreatmentsCount { get; set; }

        public int CompletedTasksCount { get; set; }

        public int PlannedTasksCount { get; set; }

        public int WateringsCount { get; set; }

        public int FertilizingsCount { get; set; }

        public int SprayingsCount { get; set; }

        public int HarvestsCount { get; set; }

        public decimal TotalHarvest { get; set; }

        public int PhotosCount { get; set; }
    }
}
