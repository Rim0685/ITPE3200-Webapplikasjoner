// The Models folder contains the application's domain entities.
// This class represents a quiz with a title, topic and description.

namespace QuizApi.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Links this quiz to its questions through the junction entity.
        public List<QuizQuestion> QuizQuestions { get; set; }
            = new List<QuizQuestion>();

        // Contains the attempts associated with this quiz.
        public List<QuizAttempt> QuizAttempts { get; set; }
            = new List<QuizAttempt>();
    }
}