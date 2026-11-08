// The ViewModels folder contains data passed between controllers and views.
// This class represents a flashcard shown in the profile overview.

namespace Quizapp.ViewModels
{
    public class FlashcardSummaryViewModel
    {
        public int FlashcardId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}