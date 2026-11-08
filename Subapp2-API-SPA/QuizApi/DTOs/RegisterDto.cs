// The DTOs folder contains the data sent between the API and the client.
// This class holds the data a new user submits when registering.
// The password rules themselves are checked by ASP.NET Core Identity.

using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
