using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class Employee
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string CodeId { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public DateTime JoinDate { get; set; }

        public DateTime? ResignDate { get; set; }

        [StringLength(150, MinimumLength = 3)]
        public string? PlaceOfBirth { get; set; }

        [Required]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? PhoneNumber { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? Religion { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string City { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 3)]
        public string? ZipCode { get; set; }

        [Required]
        public int Gender { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string? Email { get; set; }
    }
}
