// The Models folder contains the application's domain entities.
// This class represents a deck that groups a user's flashcards.

namespace QuizApi.Models
{
    public class FlashcardDeck
    {
        public int FlashcardDeckId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Identifies the user who owns this deck.
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        // Deleting a deck also deletes the flashcards in it.
        public List<Flashcard> Flashcards { get; set; } = new();
    }
}