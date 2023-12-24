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
        public int Year { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public required string Month { get; set; }

        [StringLength(10)]
        public string Day01 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day02 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day03 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day04 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day05 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day06 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day07 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day08 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day09 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day10 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day11 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day12 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day13 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day14 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day15 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day16 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day17 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day18 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day19 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day20 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day21 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day22 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day23 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day24 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day25 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day26 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day27 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day28 { get; set; } = string.Empty;
        [StringLength(10)]
        public string Day29 { get; set; } = "D";
        [StringLength(10)]
        public string Day30 { get; set; } = "D";
        [StringLength(10)]
        public string Day31 { get; set; } = "D";

        public Guid? BatchId { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUser { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedByUser { get; set; }


    }
}
