using System.ComponentModel.DataAnnotations;

namespace Webstore.Models.ViewModels
{
    public class LoginViewModels
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
