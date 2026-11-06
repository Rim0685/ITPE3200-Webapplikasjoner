// The DTOs folder contains the data sent between the API and the client.
// This class holds the data a user submits when creating a flashcard.
// The owner is not included, because it is read from the user's token.

using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class CreateFlashcardDto
    {
        // [Required] also rejects text that only contains spaces.
        [Required(ErrorMessage = "The question is required.")]
        [StringLength(500, ErrorMessage = "The question cannot be longer than 500 characters.")]
        public string Front { get; set; } = string.Empty;

        [Required(ErrorMessage = "The answer is required.")]
        [StringLength(1000, ErrorMessage = "The answer cannot be longer than 1000 characters.")]
        public string Back { get; set; } = string.Empty;
    }
}
