using System.ComponentModel.DataAnnotations;

namespace WebClient.ViewModels.Auth
{
    public class RegisterFormRequest
    {
        [Required(ErrorMessage = "Method is required")]
        public int RegisterMethod { get; set; }

        [Required(ErrorMessage = "IdNumber is required")]
        public string? IdNumber { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
