using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class MasterAttendance
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string IdNumber { get; set; }
        public DateTime? PresentIn { get; set; }
        public DateTime? PresentOut { get; set; }
        public double TotalWorkHours { get; set; } = 0;
        public double OverTime1 { get; set; } = 0;
        public double OverTime2 { get; set; } = 0;
        public double TotalOverTime { get; set; } = 0;
        public DateTime? BreakIn { get; set; }
        public DateTime? BreakOut { get; set; }
        public double TotalBreak { get; set; } = 0;
        [StringLength(50)]
        public string TypeDay { get; set; } = string.Empty;

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public required string SourceData { get; set; }
        public string? SourceId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
