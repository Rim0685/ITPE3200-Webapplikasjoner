// The Models folder contains classes that represent application data.
// This class stores a study note belonging to a user.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.Models
{
    public class StudyNote
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
        [Display(Name = "Note")]
        public string Content { get; set; } = string.Empty;

        // Stores the creation time in UTC.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Identifies the owner and is assigned on the server.
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Provides access to the associated user.
        public ApplicationUser? User { get; set; }
    }
}