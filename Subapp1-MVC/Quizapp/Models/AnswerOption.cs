// This class is the model for one possible answer to a question.
// It stores text or an image, and whether the answer option is correct.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.Models // The class belongs to the models in the Quizapp project.
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