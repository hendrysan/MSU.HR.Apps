using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class StagingDocumentAttendanceDetail
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid StagingDocumentAttendanceId { get; set; }
        [StringLength(10, MinimumLength = 3)]
        public required string separator { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column0 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column1 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column2 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column3 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column4 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column5 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column6 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column7 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column8 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column9 { get; set; }
        [StringLength(150, MinimumLength = 3)]
        public string? Column10 { get; set; }

    }

}
