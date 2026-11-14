using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quizapp.ViewModels
{
    // Holds flashcard data used in forms and views.
    public class FlashcardViewModel
    {
        public int FlashcardId { get; set; }

        [Required(ErrorMessage = "Please enter a question.")]
        [StringLength(
            500,
            ErrorMessage = "The question cannot exceed 500 characters.")]
        [Display(Name = "Question")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter an answer.")]
        [StringLength(
            1000,
            ErrorMessage = "The answer cannot exceed 1000 characters.")]
        [Display(Name = "Answer")]
        public string AnswerText { get; set; } = string.Empty;

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Deck")]
        public int? DeckId { get; set; }

        // Displays the name of the associated deck.
        public string? DeckName { get; set; }

        [StringLength(
            100,
            ErrorMessage = "The deck name cannot exceed 100 characters.")]
        [Display(Name = "Or create a new deck")]
        public string? NewDeckName { get; set; }

        // Contains the user's available decks for the dropdown.
        public List<SelectListItem> Decks { get; set; }
            = new List<SelectListItem>();

        // Contains the current position and number of cards in study mode.
        public int StudyIndex { get; set; }

        public int StudyTotal { get; set; }
    }
}