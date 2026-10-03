// Denne klassen er modellen for ett spørsmål i en quiz.
// Den kobler spørsmålet til riktig quiz og til svaralternativene brukeren kan velge mellom.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; } = string.Empty;

        public QuestionType QuestionType { get; set; }

        public int QuizId { get; set; }

        public Quiz? Quiz { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<AnswerOption> AnswerOptions { get; set; }
            = new List<AnswerOption>();
    }
}