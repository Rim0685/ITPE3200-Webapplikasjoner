// The DAL folder contains database configuration and data access code.
// This class defines the database entities and their relationships.

using Microsoft.EntityFrameworkCore;
using QuizApi.Models;

namespace QuizApi.DAL
{
    public class QuizDbContext : DbContext
    {
        // Receives database settings through dependency injection.
        public QuizDbContext(DbContextOptions<QuizDbContext> options)
            : base(options)
        {
        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<AttemptAnswer> AttemptAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Uses both foreign keys as the junction table's primary key.
            modelBuilder.Entity<QuizQuestion>()
                .HasKey(qq => new { qq.QuizId, qq.QuestionId });

            // A quiz can contain multiple question links.
            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Quiz)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuizId);

            // A question can be linked to multiple quizzes.
            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Question)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuestionId);

            // A question owns multiple answer options.
            modelBuilder.Entity<AnswerOption>()
                .HasOne(a => a.Question)
                .WithMany(q => q.AnswerOptions)
                .HasForeignKey(a => a.QuestionId);

            // Prevents deleting a quiz that has recorded attempts.
            modelBuilder.Entity<QuizAttempt>()
                .HasOne(a => a.Quiz)
                .WithMany(q => q.QuizAttempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            // An attempt contains multiple recorded answers.
            modelBuilder.Entity<AttemptAnswer>()
                .HasOne(a => a.QuizAttempt)
                .WithMany(a => a.AttemptAnswers)
                .HasForeignKey(a => a.QuizAttemptId);

            // Prevents deleting a question used in recorded answers.
            modelBuilder.Entity<AttemptAnswer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Stores selected options in a separate many-to-many join table.
            modelBuilder.Entity<AttemptAnswer>()
                .HasMany(a => a.SelectedOptions)
                .WithMany()
                .UsingEntity("AttemptAnswerOptions");

            // Allows only one recorded answer per question in an attempt.
            modelBuilder.Entity<AttemptAnswer>()
                .HasIndex(a => new { a.QuizAttemptId, a.QuestionId })
                .IsUnique();
        }
    }
}