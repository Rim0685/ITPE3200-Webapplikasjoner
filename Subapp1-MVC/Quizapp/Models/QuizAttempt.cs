// Denne klassen er modellen for én fullført gjennomføring av en quiz.
// Den brukes til å lagre resultatet, tidspunktet og hvilke svar brukeren valgte.

namespace Quizapp.Models
{
    public class QuizAttempt
    {
        public int QuizAttemptId { get; set; }

        public int Score { get; set; }

        public int TotalQuestions { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public int QuizId { get; set; }

        public Quiz? Quiz { get; set; }

        public ICollection<AttemptAnswer> AttemptAnswers { get; set; }
            = new List<AttemptAnswer>();
    }
}