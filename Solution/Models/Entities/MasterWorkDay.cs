using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
    public class MasterWorkDay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required DateTime DateWork { get; set; }

        [Required]
        public required bool IsHoliday { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUser { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedByUser { get; set; }


    }
}
