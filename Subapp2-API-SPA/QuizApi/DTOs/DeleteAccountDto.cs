// The DTOs folder contains the data sent between the API and the client.
// This class holds the password that confirms an account deletion.

using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class DeleteAccountDto
    {
        [Required(ErrorMessage = "The password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
