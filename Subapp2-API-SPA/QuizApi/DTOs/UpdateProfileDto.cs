// The DTOs folder contains the data sent between the API and the client.
// This class holds the fields a user can change on their profile.

using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "The first name is required.")]
        [StringLength(100, ErrorMessage = "The first name cannot be longer than 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The last name is required.")]
        [StringLength(100, ErrorMessage = "The last name cannot be longer than 100 characters.")]
        public string LastName { get; set; } = string.Empty;
    }
}
