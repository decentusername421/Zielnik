using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class CreateUserDto
    {
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć minimum 6 znaków")]
        public string Password { get; set; } = string.Empty;
    }
}
