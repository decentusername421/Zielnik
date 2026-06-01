using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
