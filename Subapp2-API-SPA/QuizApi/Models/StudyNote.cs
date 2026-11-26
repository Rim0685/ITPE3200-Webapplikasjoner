// The Models folder contains the application's domain entities.
// This class represents a personal study note with a title and content.

namespace QuizApi.Models
{
    public class StudyNote
    {
        public int StudyNoteId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        // Stores the creation time in UTC.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Identifies the user who owns this note. Set on the server from the token.
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}
