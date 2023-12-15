using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class DocumentAttendanceDetail
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid DocumentAttendanceId { get; set; }
        public required string separator { get; set; }
        public string? Column0 { get; set; }
        public string? Column1 { get; set; }
        public string? Column2 { get; set; }
        public string? Column3 { get; set; }
        public string? Column4 { get; set; }
        public string? Column5 { get; set; }
        public string? Column6 { get; set; }
        public string? Column7 { get; set; }
        public string? Column8 { get; set; }
        public string? Column9 { get; set; }
        public string? Column10 { get; set; }

    }

}
