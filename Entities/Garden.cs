using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Zielnik.Entities
{
    public class Garden
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? UserId { get; set; }

        public IdentityUser? User { get; set; }

        public List<UserPlant> Plants { get; set; } = new();
    }
}