// This class is the model for a flashcard.
// It stores the question on the front and the answer on the back of the card.

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
    }
}