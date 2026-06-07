using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Zielnik.Entities
{
    public class PlantCategory
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = true;

        public string? CreatedByUserId { get; set; }

        public IdentityUser? CreatedByUser { get; set; }

        public List<Plant> Plants { get; set; } = new();
    }
}
