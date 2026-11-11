using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    // Implements database operations for quizzes and quiz attempts.
    public class QuizRepository : IQuizRepository
    {
        private readonly QuizDbContext _db;
        private readonly ILogger<QuizRepository> _logger;

        // Receives the database context and logger through dependency injection.
        public QuizRepository(
            QuizDbContext db,
            ILogger<QuizRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Retrieves all quizzes.
        public async Task<IEnumerable<Quiz>?> GetAll()
        {
            try
            {
                return await _db.Quizzes
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to retrieve quizzes.");

                return null;
            }
        }

        // Retrieves a quiz with its questions and answer options.
        public async Task<Quiz?> GetQuizById(int id)
        {
            try
            {
                return await _db.Quizzes
                    .Include(quiz => quiz.Questions)
                        .ThenInclude(question => question.AnswerOptions)
                    .FirstOrDefaultAsync(quiz => quiz.QuizId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to retrieve QuizId {QuizId}.",
                    id);

                return null;
            }
        }

        // Saves a new quiz.
        public async Task<bool> Create(Quiz quiz)
        {
            try
            {
                _db.Quizzes.Add(quiz);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to create a quiz.");

                return false;
            }
        }

        // Saves changes to an existing quiz.
        public async Task<bool> Update(Quiz quiz)
        {
            try
            {
                _db.Quizzes.Update(quiz);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to update QuizId {QuizId}.",
                    quiz.QuizId);

                return false;
            }
        }

        // Deletes a quiz if it exists.
        public async Task<bool> Delete(int id)
        {
            try
            {
                var quiz = await _db.Quizzes.FindAsync(id);

                if (quiz == null)
                {
                    _logger.LogWarning(
                        "[QuizRepository] QuizId {QuizId} was not found for deletion.",
                        id);

                    return false;
                }

                _db.Quizzes.Remove(quiz);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to delete QuizId {QuizId}.",
                    id);

                return false;
            }
        }

        // Saves a completed quiz attempt.
        public async Task<bool> AddQuizAttempt(QuizAttempt quizAttempt)
        {
            try
            {
                _db.QuizAttempts.Add(quizAttempt);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to save an attempt for QuizId {QuizId}.",
                    quizAttempt.QuizId);

                return false;
            }
        }

        // Retrieves an attempt with data used by the result and solution pages.
        public async Task<QuizAttempt?> GetQuizAttemptById(int id)
        {
            try
            {
                return await _db.QuizAttempts
                    .Include(attempt => attempt.Quiz)
                    .Include(attempt => attempt.AttemptAnswers)
                        .ThenInclude(answer => answer.AnswerOption)
                            .ThenInclude(option => option!.Question)
                                .ThenInclude(question => question!.AnswerOptions)
                    .FirstOrDefaultAsync(attempt =>
                        attempt.QuizAttemptId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to retrieve QuizAttemptId {QuizAttemptId}.",
                    id);

                return null;
            }
        }

        // Retrieves the latest completed attempt for a quiz across all users.
        public async Task<QuizAttempt?> GetLatestQuizAttempt(int quizId)
        {
            try
            {
                return await _db.QuizAttempts
                    .Include(attempt => attempt.AttemptAnswers)
                    .Where(attempt => attempt.QuizId == quizId)
                    .OrderByDescending(attempt => attempt.CompletedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to retrieve the latest attempt for QuizId {QuizId}.",
                    quizId);

                return null;
            }
        }

        // Counts completed attempts for a quiz across all users.
        public async Task<int?> GetQuizAttemptCount(int quizId)
        {
            try
            {
                return await _db.QuizAttempts
                    .CountAsync(attempt => attempt.QuizId == quizId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to count attempts for QuizId {QuizId}.",
                    quizId);

                return null;
            }
        }

        // Retrieves completed quiz attempts belonging to the specified user.
        public async Task<IEnumerable<QuizAttempt>?> GetUserQuizAttempts(
            string userId)
        {
            try
            {
                return await _db.QuizAttempts
                    .AsNoTracking()
                    .Include(attempt => attempt.Quiz)
                    .Where(attempt => attempt.UserId == userId)
                    .OrderByDescending(attempt => attempt.CompletedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to retrieve the user's quiz attempts.");

                return null;
            }
        }

        // Deletes an attempt only if it belongs to the specified user.
        public async Task<bool> DeleteUserQuizAttempt(
            int attemptId, string userId)
        {
            try
            {
                var attempt = await _db.QuizAttempts
                    .Include(attempt => attempt.AttemptAnswers)
                    .FirstOrDefaultAsync(attempt =>
                        attempt.QuizAttemptId == attemptId
                        && attempt.UserId == userId);

                if (attempt == null)
                {
                    _logger.LogWarning(
                        "[QuizRepository] QuizAttemptId {QuizAttemptId} was not available for deletion.",
                        attemptId);

                    return false;
                }

                // Deletes the recorded answers and their associated attempt.
                _db.AttemptAnswers.RemoveRange(attempt.AttemptAnswers);
                _db.QuizAttempts.Remove(attempt);

                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[QuizRepository] Failed to delete QuizAttemptId {QuizAttemptId}.",
                    attemptId);

                return false;
            }
        }
    }
}