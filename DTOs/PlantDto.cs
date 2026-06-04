namespace Zielnik.DTOs
{
    public class PlantDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Species { get; set; } = string.Empty;

        public int WateringFrequencyDays { get; set; }

        public List<string> Gardens { get; set; } = new();

        public List<string> Categories { get; set; } = new();
    }
}