// The Models folder contains the application's domain entities.
// This class represents a text or image answer option for a question.

namespace QuizApi.Models
{
    public class AnswerOption
    {
        public int AnswerOptionId { get; set; }

        public string Text { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsCorrect { get; set; }

        // Identifies the question that owns this answer option.
        public int QuestionId { get; set; }

        public Question? Question { get; set; }
    }
}