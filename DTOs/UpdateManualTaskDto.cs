using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class UpdateManualTaskDto
    {
        [Required]
        public Guid UserPlantId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TaskType { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }
    }
}
