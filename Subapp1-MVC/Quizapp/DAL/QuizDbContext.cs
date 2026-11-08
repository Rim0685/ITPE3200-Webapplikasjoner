// The DAL folder contains database configuration and data access code.
// This context manages application data and Identity user tables.

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    public class QuizDbContext : IdentityDbContext<ApplicationUser>
    {
        // Receives database settings through dependency injection.
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

        public DbSet<StudyNote> StudyNotes { get; set; }
    }
}