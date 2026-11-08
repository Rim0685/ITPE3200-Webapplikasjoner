// The ViewModels folder contains data passed between controllers and views.
// This class represents a completed quiz attempt shown on the profile page.

namespace Quizapp.ViewModels
{
    public class QuizAttemptViewModel
    {
        public int QuizAttemptId { get; set; }

        public string QuizTitle { get; set; } = string.Empty;

        public int Score { get; set; }

        public int TotalQuestions { get; set; }

        public DateTime CompletedAt { get; set; }
    }
}