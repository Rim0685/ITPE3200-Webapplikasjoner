// This class is the model for one completed attempt at a quiz.
// It stores the result, the time and which answers the user selected.

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

        public ICollection<AttemptAnswer> AttemptAnswers { get; set; }
            = new List<AttemptAnswer>();
    }
}