namespace Zielnik.DTOs
{
    public class TaskDto
    {
        public Guid UserPlantId { get; set; }

        public string PlantName { get; set; } = string.Empty;

        public string? Nickname { get; set; }

        public string TaskType { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }
    }
}