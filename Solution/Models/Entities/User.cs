﻿using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
    }
}
