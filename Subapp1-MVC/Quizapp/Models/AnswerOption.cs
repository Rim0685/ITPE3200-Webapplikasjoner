// This class represents a single possible answer to a question.
// It is used to store either text or an image, along with information indicating whether the answer

using System.ComponentModel.DataAnnotations;

namespace Quizapp.Models // Indicates that this class belongs to the models in the Quizapp project.
{
    public class AnswerOption
    {
        public int AnswerOptionId { get; set; }

        [StringLength(500)]
        public string? AnswerText { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }

        public Question? Question { get; set; }

        public ICollection<AttemptAnswer> AttemptAnswers { get; set; } 
        = new List<AttemptAnswer>();
    }
}