// The Models folder contains the application's domain entities.
// This class stores a completed quiz attempt and its calculated result.

namespace QuizApi.Models
{
    public class QuizAttempt
    {
        public int QuizAttemptId { get; set; }

        public int QuizId { get; set; }

        public Quiz? Quiz { get; set; }

        // Identifies the user who completed the attempt.
        public string UserId { get; set; } = string.Empty;

        public DateTime CompletedAt { get; set; }

        // Stores the awarded points, including partial points.
        public decimal Score { get; set; }

        public decimal MaxScore { get; set; }

        // Contains the recorded answers for this attempt.
        public List<AttemptAnswer> AttemptAnswers { get; set; }
            = new List<AttemptAnswer>();
    }
}