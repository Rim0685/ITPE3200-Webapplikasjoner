// The DTOs folder contains the data sent between the API and the client.
// These classes describe a flashcard deck. UserId is left out on purpose.

using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class FlashcardDeckDto
    {
        public int FlashcardDeckId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CardCount { get; set; }

        // Only filled when a single deck is fetched.
        public List<FlashcardDto> Flashcards { get; set; } = new();
    }

    public class CreateFlashcardDeckDto
    {
        [Required(ErrorMessage = "The deck name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
        public string? Description { get; set; }
    }

    public class UpdateFlashcardDeckDto
    {
        [Required(ErrorMessage = "The deck name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
        public string? Description { get; set; }
    }
}