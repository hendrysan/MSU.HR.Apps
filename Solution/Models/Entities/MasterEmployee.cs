using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class MasterEmployee
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string? IdNumber { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string? FullName { get; set; }

        //[Required]
        //public DateTime? JoinDate { get; set; }

        //public DateTime? ResignDate { get; set; }

        //[StringLength(150, MinimumLength = 3)]
        //public string? PlaceOfBirth { get; set; }

        //[Required]
        //public DateTime? DateOfBirth { get; set; }

        //[StringLength(50, MinimumLength = 3)]
        //public string? PhoneNumber { get; set; }

        //[StringLength(50, MinimumLength = 3)]
        //public string? Religion { get; set; }

        //[Required]
        //[StringLength(250, MinimumLength = 3)]
        //public string? Address { get; set; }

        //[Required]
        //[StringLength(250, MinimumLength = 3)]
        //public string? City { get; set; }

        //[StringLength(50, MinimumLength = 3)]
        //public string? ZipCode { get; set; }

        //[Required]
        //public int Gender { get; set; } = 0;

        //[StringLength(250, MinimumLength = 3)]
        //public string? Email { get; set; }
    }
}
