// Denne klassen er modellen for ett mulig svar på et spørsmål.
// Den brukes til å lagre tekst eller bilde og informasjon om svaralternativet er riktig.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.Models // Angir at klassen tilhører modellene i Quizapp-prosjektet.
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