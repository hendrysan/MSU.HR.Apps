using System.ComponentModel.DataAnnotations;

namespace WebClient.ViewModels.Auth
{
    public class LoginFormRequest
    {
        [Required(ErrorMessage = "This field is required")]
        public string? UserInput { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
