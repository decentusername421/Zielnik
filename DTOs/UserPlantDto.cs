using Zielnik.Entities;

public class UserPlantDto
{
    public Guid Id { get; set; }

    public string? Nickname { get; set; }

    public string PlantName { get; set; } = string.Empty;

    public string GardenName { get; set; } = string.Empty;

    public PlantStatus Status { get; set; }

    public DateTime? SowingDate { get; set; }

    public DateTime? PlantingDate { get; set; }
}