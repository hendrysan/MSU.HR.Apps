using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class MasterShift
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public required string Name { get; set; }
        [Required]
        public required TimeSpan WorkIn { get; set; }
        [Required]
        public required TimeSpan WorkOut { get; set; }

        [Required]
        public required TimeSpan Break1In { get; set; }
        [Required]
        public required TimeSpan Break1Out { get; set; }
        [Required]
        public required TimeSpan Break2In { get; set; }
        [Required]
        public required TimeSpan Break2Out { get; set; }

    }
}
