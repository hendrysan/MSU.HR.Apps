using System.ComponentModel.DataAnnotations;
using static Models.Entities.EnumEntities;

namespace Models.Entities
{
    public class StagingDocumentAttendance
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public required string DocumentName { get; set; }
        public required string Path { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public required string Size { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public required string Type { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Extension { get; set; }
        [Required]
        public required EnumStatusDocumentAttendance Status { get; set; }
        [StringLength(250, MinimumLength = 3)]
        public string? Remarks { get; set; }
        [Required]
        public Guid CreatedByUser { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public required DateTime DocumentDate { get; set; }
        [Required]
        public required long TotalRow { get; set; }

        public List<StagingDocumentAttendanceDetail>? Details { get; set; }
    }
}
