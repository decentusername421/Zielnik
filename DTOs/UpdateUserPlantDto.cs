using Zielnik.Entities;

namespace Zielnik.DTOs
{
    public class UpdateUserPlantDto
    {
        public string? Nickname { get; set; }

        public DateTime? SowingDate { get; set; }

        public DateTime? PlantingDate { get; set; }

        public PlantStatus Status { get; set; }
    }
}