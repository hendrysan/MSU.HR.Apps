using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebClient.ViewModels.Auth
{
    public class RegisterFormRequest
    {
        [Required(ErrorMessage = "Method is required")]
        public int RegisterMethod { get; set; }

        [Required(ErrorMessage = "IdNumber is required")]
        public string? IdNumber { get; set; }


        [Required(ErrorMessage = "Full Name is required")]
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        public string? Repassword { get; set; }

        public List<SelectListItem>? DropdownMethod { get; set; }
    }
}
