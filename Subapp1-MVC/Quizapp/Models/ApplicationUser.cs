// The Models folder contains classes that represent application data.
// This class extends the Identity user with editable profile information.

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Quizapp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Stores the name displayed on the website.
        [StringLength(50)]
        public string DisplayName { get; set; } = string.Empty;

        // Stores the user's optional full name.
        [StringLength(100)]
        public string? FullName { get; set; }

        // Stores an optional short description of the user.
        [StringLength(500)]
        public string? AboutMe { get; set; }
    }
}