using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL // Angir at klassen tilhører applikasjonens datatilgangslag.
{
    // Denne klassen representerer forbindelsen mellom applikasjonen og databasen.
    // Hver DbSet-egenskap representerer en tabell som Entity Framework skal opprette.
    public class QuizDbContext : DbContext
    {
        public QuizDbContext(DbContextOptions<QuizDbContext> options)
            : base(options)
        {
        }

        public DbSet<Quiz> Quizzes { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<AnswerOption> AnswerOptions { get; set; }

        public DbSet<QuizAttempt> QuizAttempts { get; set; }

        public DbSet<AttemptAnswer> AttemptAnswers { get; set; }

        public DbSet<Flashcard> Flashcards { get; set; }
    }
}