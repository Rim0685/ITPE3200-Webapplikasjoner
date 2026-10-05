using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    // Handles database operations defined by IQuizRepository.
    public class QuizRepository : IQuizRepository
    {
        private readonly QuizDbContext _db;

        public QuizRepository(QuizDbContext db)
        {
            _db = db;
        }

        // Retrieves all quizzes without tracking them for changes.
        public async Task<IEnumerable<Quiz>> GetAll()
        {
            return await _db.Quizzes
                .AsNoTracking()
                .ToListAsync();
        }

        // Retrieves one quiz together with its questions and answer options.
        public async Task<Quiz?> GetQuizById(int id)
        {
            return await _db.Quizzes
                .Include(quiz => quiz.Questions)
                    .ThenInclude(question => question.AnswerOptions)
                .FirstOrDefaultAsync(quiz => quiz.QuizId == id);
        }

        // Creates a new quiz and saves it to the database.
        public async Task Create(Quiz quiz)
        {
            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();
        }

        // Update and exisiting quiz and saves the changes.
        public async Task Update(Quiz quiz)
        {
            _db.Quizzes.Update(quiz);
            await _db.SaveChangesAsync();
        }

        // Deletes a quiz by id. Returns flase if the quiz does not exist.
        public async Task<bool> Delete(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);

            if (quiz == null)
            {
                return false;
            }

            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();

            return true;
        }

        // Saves a completed quiz attempt to the database.
        public async Task AddQuizAttempt(QuizAttempt quizAttempt)
        {
            _db.QuizAttempts.Add(quizAttempt);
            await _db.SaveChangesAsync();
        }
        
        // Retrieves a quiz attempt with the related quiz, submitted answers,
        // questions, and answer options needed by the result and solution pages.
        public async Task<QuizAttempt?> GetQuizAttemptById(int id)
        {
            return await _db.QuizAttempts
                .Include(attempt => attempt.Quiz)
                .Include(attempt => attempt.AttemptAnswers)
                    .ThenInclude(attemptAnswer => attemptAnswer.AnswerOption)
                        .ThenInclude(answerOption => answerOption!.Question)
                            .ThenInclude(question => question!.AnswerOptions)
                .FirstOrDefaultAsync(attempt => attempt.QuizAttemptId == id);
        }

        // Retrieves the most recently completed attempt for a quiz.
        public async Task<QuizAttempt?> GetLatestQuizAttempt(int quizId)
        {
            return await _db.QuizAttempts
                .Include(attempt => attempt.AttemptAnswers)
                .Where(attempt => attempt.QuizId == quizId)
                .OrderByDescending(attempt => attempt.CompletedAt)
                .FirstOrDefaultAsync();
        }

        // Returns the number of attempts made for a specific quiz.
        public async Task<int> GetQuizAttemptCount(int quizId)
        {
            return await _db.QuizAttempts
                .CountAsync(attempt => attempt.QuizId == quizId);
        }
    }
}