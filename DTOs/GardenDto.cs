namespace Zielnik.DTOs
{
    public class GardenDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<string> Plants { get; set; } = new();
    }
}