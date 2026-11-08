// The DAL folder contains database configuration and data access code.
// This class implements the database operations defined in IQuizRepository.

using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    public class QuizRepository : IQuizRepository
    {
        private readonly QuizDbContext _db;

        // Receives the database context through dependency injection.
        public QuizRepository(QuizDbContext db)
        {
            _db = db;
        }

        // Retrieves all quizzes.
        public async Task<IEnumerable<Quiz>> GetAll()
        {
            return await _db.Quizzes
                .AsNoTracking()
                .ToListAsync();
        }

        // Retrieves a quiz with its questions and answer options.
        public async Task<Quiz?> GetQuizById(int id)
        {
            return await _db.Quizzes
                .Include(quiz => quiz.Questions)
                    .ThenInclude(question => question.AnswerOptions)
                .FirstOrDefaultAsync(quiz => quiz.QuizId == id);
        }

        // Saves a new quiz.
        public async Task Create(Quiz quiz)
        {
            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();
        }

        // Saves changes to an existing quiz.
        public async Task Update(Quiz quiz)
        {
            _db.Quizzes.Update(quiz);
            await _db.SaveChangesAsync();
        }

        // Deletes a quiz if it exists.
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

        // Saves a completed quiz attempt.
        public async Task AddQuizAttempt(QuizAttempt quizAttempt)
        {
            _db.QuizAttempts.Add(quizAttempt);
            await _db.SaveChangesAsync();
        }

        // Retrieves an attempt with data used by the result and solution pages.
        public async Task<QuizAttempt?> GetQuizAttemptById(int id)
        {
            return await _db.QuizAttempts
                .Include(attempt => attempt.Quiz)
                .Include(attempt => attempt.AttemptAnswers)
                    .ThenInclude(attemptAnswer => attemptAnswer.AnswerOption)
                        .ThenInclude(answerOption => answerOption!.Question)
                            .ThenInclude(question => question!.AnswerOptions)
                .FirstOrDefaultAsync(attempt =>
                    attempt.QuizAttemptId == id);
        }

        // Retrieves the latest completed attempt for a quiz.
        public async Task<QuizAttempt?> GetLatestQuizAttempt(int quizId)
        {
            return await _db.QuizAttempts
                .Include(attempt => attempt.AttemptAnswers)
                .Where(attempt => attempt.QuizId == quizId)
                .OrderByDescending(attempt => attempt.CompletedAt)
                .FirstOrDefaultAsync();
        }

        // Counts completed attempts for a quiz.
        public async Task<int> GetQuizAttemptCount(int quizId)
        {
            return await _db.QuizAttempts
                .CountAsync(attempt => attempt.QuizId == quizId);
        }

        // Retrieves completed quiz attempts belonging to the specified user.
        public async Task<IEnumerable<QuizAttempt>> GetUserQuizAttempts(
            string userId)
        {
            return await _db.QuizAttempts
                .AsNoTracking()
                .Include(attempt => attempt.Quiz)
                .Where(attempt => attempt.UserId == userId)
                .OrderByDescending(attempt => attempt.CompletedAt)
                .ToListAsync();
        }

        // Deletes an attempt only if it belongs to the specified user.
        public async Task<bool> DeleteUserQuizAttempt(
            int attemptId, string userId)
        {
            var attempt = await _db.QuizAttempts
                .Include(attempt => attempt.AttemptAnswers)
                .FirstOrDefaultAsync(attempt =>
                    attempt.QuizAttemptId == attemptId
                    && attempt.UserId == userId);

            if (attempt == null)
            {
                return false;
            }

            // Deletes the recorded answers and their associated attempt.
            _db.AttemptAnswers.RemoveRange(attempt.AttemptAnswers);
            _db.QuizAttempts.Remove(attempt);

            await _db.SaveChangesAsync();

            return true;
        }
    }
}