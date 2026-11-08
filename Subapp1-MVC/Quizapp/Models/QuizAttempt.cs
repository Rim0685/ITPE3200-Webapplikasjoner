// The Models folder contains classes that represent application data.
// This class stores a completed quiz attempt and its associated user.

namespace Quizapp.Models
{
    public class QuizAttempt
    {
        public int QuizAttemptId { get; set; }

        public int Score { get; set; }

        public int TotalQuestions { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public int QuizId { get; set; }

        public Quiz? Quiz { get; set; }

        // Identifies the user who completed the quiz.
        // Nullable to preserve existing attempts without an assigned user.
        public string? UserId { get; set; }

        // Provides access to the associated user.
        public ApplicationUser? User { get; set; }

        public ICollection<AttemptAnswer> AttemptAnswers { get; set; }
            = new List<AttemptAnswer>();
    }
}