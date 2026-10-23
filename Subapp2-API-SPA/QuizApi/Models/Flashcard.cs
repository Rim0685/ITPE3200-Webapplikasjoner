// The Models folder contains the application's domain entities.
// This class represents a personal flashcard with a front and a back.

namespace QuizApi.Models
{
    public class Flashcard
    {
        public int FlashcardId { get; set; }

        public string Front { get; set; } = string.Empty;

        public string Back { get; set; } = string.Empty;

        // Identifies the user who owns this flashcard.
        public string UserId { get; set; } = string.Empty;
    }
}