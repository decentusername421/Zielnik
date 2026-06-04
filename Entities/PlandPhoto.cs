using Zielnik.Entities;

public class PlantPhoto
{
    public Guid Id { get; set; }

    public Guid UserPlantId { get; set; }

    public UserPlant UserPlant { get; set; } = null!;

    public string FilePath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}