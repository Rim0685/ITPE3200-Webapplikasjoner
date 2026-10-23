// This class is the primary model for a quiz.
// It is used to store quiz information and link the quiz to its questions and quiz attempts.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Question> Questions { get; set; }
            = new List<Question>();

        public ICollection<QuizAttempt> QuizAttempts { get; set; }
            = new List<QuizAttempt>();
    }
}