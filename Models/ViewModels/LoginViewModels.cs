using System.ComponentModel.DataAnnotations;

namespace Webstore.Models.ViewModels
{
    public class LoginViewModels
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        public string? Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
