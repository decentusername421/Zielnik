using System.ComponentModel.DataAnnotations;

namespace Zielnik.DTOs
{
    public class AutomaticTaskDto
    {
        [Required]
        public Guid UserPlantId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TaskType { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }
    }

    public class RescheduleAutomaticTaskDto : AutomaticTaskDto
    {
        [Required]
        public DateTime NewDueDate { get; set; }
    }
}
