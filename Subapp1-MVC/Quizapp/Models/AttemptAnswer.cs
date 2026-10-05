// This class acts as a junction model between a quiz attempt and a selected answer option.
// It enables multiple selected answers to be stored within the same quiz attempt.
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