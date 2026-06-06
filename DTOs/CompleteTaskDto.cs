public class CompleteTaskDto
{
    public Guid UserPlantId { get; set; }

    public string TaskType { get; set; } = string.Empty;

    public string? Notes { get; set; }
}