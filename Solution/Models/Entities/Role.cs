using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
    }
}
