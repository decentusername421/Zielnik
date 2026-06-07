using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class CreateGardenDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}