// The DAL folder contains database configuration and data access code.
// This class saves and retrieves completed quiz attempts.
// Scoring and solution access rules belong to the evaluation service.

using Microsoft.EntityFrameworkCore;
using QuizApi.Models;

namespace QuizApi.DAL
{
    public class QuizAttemptRepository : IQuizAttemptRepository
    {
        private readonly QuizDbContext _db;
        private readonly ILogger<QuizAttemptRepository> _logger;

        public QuizAttemptRepository(
            QuizDbContext db,
            ILogger<QuizAttemptRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Saves an attempt with the scores provided by the service.
        public async Task<bool> CreateAsync(QuizAttempt attempt)
        {
            try
            {
                // Collects the IDs of existing selected answer options.
                var selectedOptionIds = attempt.AttemptAnswers
                    .SelectMany(a => a.SelectedOptions)
                    .Select(o => o.AnswerOptionId)
                    .Distinct()
                    .ToList();

                // Loads existing options to avoid inserting duplicate options.
                var selectedOptions = await _db.AnswerOptions
                    .Where(o => selectedOptionIds.Contains(o.AnswerOptionId))
                    .ToListAsync();

                if (selectedOptions.Count != selectedOptionIds.Count)
                {
                    _logger.LogWarning(
                        "Cannot save attempt: selected answer options were not found.");

                    return false;
                }

                // Copies attempt data without inserting existing quizzes or questions.
                var newAttempt = new QuizAttempt
                {
                    QuizId = attempt.QuizId,
                    UserId = attempt.UserId,
                    CompletedAt = attempt.CompletedAt,
                    Score = attempt.Score,
                    MaxScore = attempt.MaxScore
                };

                foreach (var answer in attempt.AttemptAnswers)
                {
                    var answerOptionIds = answer.SelectedOptions
                        .Select(o => o.AnswerOptionId)
                        .ToList();

                    newAttempt.AttemptAnswers.Add(new AttemptAnswer
                    {
                        QuestionId = answer.QuestionId,
                        AwardedPoints = answer.AwardedPoints,
                        SelectedOptions = selectedOptions
                            .Where(o => answerOptionIds.Contains(o.AnswerOptionId))
                            .ToList()
                    });
                }

                // Saves the attempt, its answers and selected-option links together.
                _db.QuizAttempts.Add(newAttempt);
                await _db.SaveChangesAsync();

                // Makes the generated attempt ID available to the caller.
                attempt.QuizAttemptId = newAttempt.QuizAttemptId;

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to save attempt for quiz {QuizId}.",
                    attempt.QuizId);

                return false;
            }
        }

        // Retrieves an attempt only if it belongs to the specified user.
        public async Task<QuizAttempt?> GetByIdAsync(
            int quizAttemptId, string userId)
        {
            try
            {
                return await _db.QuizAttempts
                    .Include(a => a.AttemptAnswers)
                        .ThenInclude(a => a.SelectedOptions)
                    .FirstOrDefaultAsync(a =>
                        a.QuizAttemptId == quizAttemptId &&
                        a.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve attempt {QuizAttemptId}.",
                    quizAttemptId);

                throw;
            }
        }

        // Retrieves the user's latest completed attempt for a quiz.
        public async Task<QuizAttempt?> GetLatestAsync(
            int quizId, string userId)
        {
            try
            {
                return await _db.QuizAttempts
                    .Include(a => a.AttemptAnswers)
                        .ThenInclude(a => a.SelectedOptions)
                    .Where(a =>
                        a.QuizId == quizId &&
                        a.UserId == userId)
                    .OrderByDescending(a => a.CompletedAt)
                    .ThenByDescending(a => a.QuizAttemptId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve latest attempt for quiz {QuizId}.",
                    quizId);

                throw;
            }
        }

        // Counts the user's saved attempts for a quiz.
        public async Task<int> CountAsync(
            int quizId, string userId)
        {
            try
            {
                return await _db.QuizAttempts
                    .CountAsync(a =>
                        a.QuizId == quizId &&
                        a.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to count attempts for quiz {QuizId}.",
                    quizId);

                throw;
            }
        }
    }
}