namespace Zielnik.DTOs
{
    public class TaskCompletionDto
    {
        public Guid? TaskId { get; set; }

        public Guid UserPlantId { get; set; }
        public string TaskType { get; set; } = string.Empty;
    }
}
