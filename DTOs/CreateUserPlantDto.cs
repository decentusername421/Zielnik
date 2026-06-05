namespace Zielnik.DTOs
{
    public class CreateUserPlantDto
    {
        public Guid PlantId { get; set; }

        public Guid GardenId { get; set; }

        public DateTime? SowingDate { get; set; }

        public DateTime? PlantingDate { get; set; }

        public string? Nickname { get; set; }

        public int? HarvestReminderDays { get; set; }
    }
}