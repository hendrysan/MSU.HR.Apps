using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class DocumentAttendance
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public required string DocumentName { get; set; }
        public required string Path { get; set; }
        [Required]
        public required string Size { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public required string Type { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Extension { get; set; }
        [Required]
        public required string Status { get; set; }
        public string? Remarks { get; set; }
        [Required]
        public Guid CreatedByUser { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public required DateTime DocumentDate { get; set; }

        public List<DocumentAttendanceDetail>? Details { get; set; }
    }
}
