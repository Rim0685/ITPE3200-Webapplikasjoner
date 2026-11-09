// Holds flashcard data used in forms and views.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.ViewModels
{
    public class FlashcardViewModel
    {
        public int FlashcardId { get; set; }

        [Required(ErrorMessage = "Please enter a question.")]
        [StringLength(500,
            ErrorMessage = "The question cannot exceed 500 characters.")]
        [Display(Name = "Question")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter an answer.")]
        [StringLength(1000,
            ErrorMessage = "The answer cannot exceed 1000 characters.")]
        [Display(Name = "Answer")]
        public string AnswerText { get; set; } = string.Empty;

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }
    }
}