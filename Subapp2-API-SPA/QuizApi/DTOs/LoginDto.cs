// The DTOs folder contains the data sent between the API and the client.
// This class holds the credentials a user submits when logging in.

using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
