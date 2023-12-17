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
        public decimal TotalWorkHours { get; set; } = 0;
        public decimal OverTime1 { get; set; } = 0;
        public decimal OverTime2 { get; set; } = 0;
        public decimal TotalOverTime { get; set; } = 0;
        public DateTime? BreakIn { get; set; }
        public DateTime? BreakOut { get; set; }
        public decimal TotalBreak { get; set; } = 0;

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public required string SourceData { get; set; }
        public string? SourceId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
