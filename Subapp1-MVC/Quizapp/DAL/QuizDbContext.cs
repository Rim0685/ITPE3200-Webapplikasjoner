using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL // Indicates that the class belongs to the application's data access layer.
{
    // Represents the connection between the application and the database.
    // Each DbSet property represents a table managed by Entity Framework.
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