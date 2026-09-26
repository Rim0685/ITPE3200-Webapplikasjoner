// Denne klassen er modellen for et flashcard.
// Den lagrer spørsmålet på forsiden og svaret på baksiden av kortet.

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