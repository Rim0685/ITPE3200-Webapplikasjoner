// The Models folder contains the application's domain entities.
// This class stores an answer to one question within a quiz attempt.

namespace QuizApi.Models
{
    public class AttemptAnswer
    {
        public int AttemptAnswerId { get; set; }

        public int QuizAttemptId { get; set; }

        public QuizAttempt? QuizAttempt { get; set; }

        public int QuestionId { get; set; }

        public Question? Question { get; set; }

        // Stores all options selected for this question.
        public List<AnswerOption> SelectedOptions { get; set; }
            = new List<AnswerOption>();

        // Stores the points awarded for this answer.
        public decimal AwardedPoints { get; set; }
    }
}