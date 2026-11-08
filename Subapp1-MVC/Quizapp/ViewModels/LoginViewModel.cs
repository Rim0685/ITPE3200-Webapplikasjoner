// The ViewModels folder contains data passed between controllers and views.
// This class defines the fields and validation rules for the login form.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        // Preserves the requested destination after a successful login.
        public string? ReturnUrl { get; set; }
    }
}