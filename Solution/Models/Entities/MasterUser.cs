﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
    [Table("MasterUsers")]
    public class MasterUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string? FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(250, MinimumLength = 3)]
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; } = false;
        public string? IdNumber { get; set; }
        [StringLength(250, MinimumLength = 3)]
        public string? PasswordHash { get; set; }
        [StringLength(250, MinimumLength = 3)]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public MasterRole? Role { get; set; }
    }
}
