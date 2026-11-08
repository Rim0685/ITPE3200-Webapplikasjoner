// The ViewModels folder contains data passed between controllers and views.
// This class represents a study note in forms and views.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.ViewModels
{
    public class StudyNoteViewModel
    {
        public int StudyNoteId { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        [StringLength(100,
            ErrorMessage = "The title cannot exceed 100 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the note.")]
        [StringLength(5000,
            ErrorMessage = "The note cannot exceed 5000 characters.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Note")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }
    }
}