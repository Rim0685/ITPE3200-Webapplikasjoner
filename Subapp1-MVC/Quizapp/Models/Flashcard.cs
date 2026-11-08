// The Models folder contains classes that represent application data.
// This class stores a flashcard and its associated user.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.Models
{
    public class Flashcard
    {
        public int FlashcardId { get; set; }

        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string AnswerText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Identifies the user who created the flashcard.
        // Nullable to preserve existing cards without an assigned user.
        public string? UserId { get; set; }

        // Provides access to the associated user.
        public ApplicationUser? User { get; set; }
    }
}