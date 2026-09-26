// Denne klassen er en koblingsmodell mellom et quizforsøk og et valgt svaralternativ.
// Den gjør det mulig å lagre flere valgte svar i samme quizforsøk.

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