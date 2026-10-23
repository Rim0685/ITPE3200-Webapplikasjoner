// The Models folder contains the application's domain entities.
// This class links quizzes and questions in a many-to-many relationship.

namespace QuizApi.Models
{
    public class QuizQuestion
    {
        public int QuizId { get; set; }

        public Quiz? Quiz { get; set; }

        public int QuestionId { get; set; }

        public Question? Question { get; set; }

        // Defines the question's position within this quiz.
        public int Order { get; set; }
    }
}