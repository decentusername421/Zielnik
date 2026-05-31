using System.ComponentModel.DataAnnotations;

namespace Zielnik.Entities
{
    public class PlantCategory
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<Plant> Plants { get; set; } = new();
    }
}