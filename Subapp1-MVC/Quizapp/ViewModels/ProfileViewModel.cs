// The ViewModels folder contains data passed between controllers and views.
// This class defines the profile form and the user's personal overviews.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Please enter a display name.")]
        [StringLength(50,
            ErrorMessage = "The display name cannot exceed 50 characters.")]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(100,
            ErrorMessage = "The full name cannot exceed 100 characters.")]
        [Display(Name = "Full name")]
        public string? FullName { get; set; }

        [StringLength(500,
            ErrorMessage = "The description cannot exceed 500 characters.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "About me")]
        public string? AboutMe { get; set; }

        // Contains the user's completed quiz attempts for the profile overview.
        public List<QuizAttemptViewModel> QuizAttempts { get; set; }
            = new List<QuizAttemptViewModel>();

        // Contains the user's flashcards for the profile overview.
        public List<FlashcardSummaryViewModel> Flashcards { get; set; }
            = new List<FlashcardSummaryViewModel>();

        // Contains the user's study notes for the profile overview.
        public List<StudyNoteViewModel> StudyNotes { get; set; }
            = new List<StudyNoteViewModel>();
    }
}