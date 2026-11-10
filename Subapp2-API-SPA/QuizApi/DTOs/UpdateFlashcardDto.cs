// The DTOs folder contains the data sent between the API and the client.
// This class holds the data a user submits when updating a flashcard.
// The flashcard's ID comes from the URL and the owner from the user's token.

using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class UpdateFlashcardDto
    {
        // [Required] also rejects text that only contains spaces.
        [Required(ErrorMessage = "The question is required.")]
        [StringLength(500, ErrorMessage = "The question cannot be longer than 500 characters.")]
        public string Front { get; set; } = string.Empty;

        [Required(ErrorMessage = "The answer is required.")]
        [StringLength(1000, ErrorMessage = "The answer cannot be longer than 1000 characters.")]
        public string Back { get; set; } = string.Empty;
        public int? DeckId { get; set; }
    }
}
