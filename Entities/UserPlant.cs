using System.ComponentModel.DataAnnotations;

namespace Zielnik.Entities
{
    public class UserPlant
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PlantId { get; set; }

        public Plant Plant { get; set; } = null!;

        [Required]
        public Guid GardenId { get; set; }

        public Garden Garden { get; set; } = null!;

        public DateTime? SowingDate { get; set; }

        public DateTime? PlantingDate { get; set; }

        [MaxLength(100)]
        public string? Nickname { get; set; }

        public PlantStatus Status { get; set; }
            = PlantStatus.Active;

        public List<PlantNote> Notes { get; set; } = new();

        public List<PlantTreatment> Treatments { get; set; } = new();

        public List<Harvest> Harvests { get; set; } = new();

        public List<PlantPhoto> Photos { get; set; } = new();

        public DateTime? NextHarvestReminder { get; set; }

        public int? HarvestReminderDays { get; set; }
    }
}