// The Models folder contains the application's domain entities.
// This class represents a question that can be used in multiple quizzes.

namespace QuizApi.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string Text { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        // Indicates whether the user can select multiple answers.
        public bool AllowMultipleAnswers { get; set; }

        // Optional image displayed with the question.
        public string? ImageUrl { get; set; }

        // Contains the available answer options for this question.
        public List<AnswerOption> AnswerOptions { get; set; }
            = new List<AnswerOption>();

        // Links this question to quizzes through the junction entity.
        public List<QuizQuestion> QuizQuestions { get; set; }
            = new List<QuizQuestion>();
    }
}