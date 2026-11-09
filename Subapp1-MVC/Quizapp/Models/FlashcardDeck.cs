// The Models folder contains classes that represent application data.
// This class stores a flashcard deck belonging to a user.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.Models
{
    public class FlashcardDeck
    {
        public int FlashcardDeckId { get; set; }

        [Required(ErrorMessage = "Please enter a name.")]
        [StringLength(100,
            ErrorMessage = "The name cannot exceed 100 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500,
            ErrorMessage = "The description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        // Stores the creation time in UTC.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Identifies the owner and is assigned on the server.
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Provides access to the associated user.
        public ApplicationUser? User { get; set; }

        // The flashcards that belong to this deck.
        public List<Flashcard> Flashcards { get; set; } = new();
    }
}