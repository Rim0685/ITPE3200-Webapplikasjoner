// This class links a quiz attempt to a selected answer option.
// It makes it possible to store several selected answers in the same quiz attempt.

namespace Quizapp.Models
{
    public class AttemptAnswer
    {
        public int AttemptAnswerId { get; set; }

        public int QuizAttemptId { get; set; }

        public QuizAttempt? QuizAttempt { get; set; }

        public int AnswerOptionId { get; set; }

        public AnswerOption? AnswerOption { get; set; }
    }
}