// The ViewModels folder contains data passed between controllers and views.
// This class represents a flashcard deck in forms and views.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.ViewModels
{
    public class FlashcardDeckViewModel
    {
        public int FlashcardDeckId { get; set; }

        [Required(ErrorMessage = "Please enter a name.")]
        [StringLength(100,
            ErrorMessage = "The name cannot exceed 100 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500,
            ErrorMessage = "The description cannot exceed 500 characters.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        // Number of cards in the deck.
        public int CardCount { get; set; }

        // The cards shown on the details page.
        public List<FlashcardSummaryViewModel> Cards { get; set; } = new();
    }
}