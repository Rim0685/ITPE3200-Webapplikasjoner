// The DAL folder contains database configuration and data access code.
// This class implements database operations for questions and answer options.

using Microsoft.EntityFrameworkCore;
using QuizApi.Models;

namespace QuizApi.DAL
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly QuizDbContext _db;
        private readonly ILogger<QuestionRepository> _logger;

        public QuestionRepository(
            QuizDbContext db,
            ILogger<QuestionRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            try
            {
                return await _db.Questions
                    .Include(q => q.AnswerOptions)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve questions.");
                throw;
            }
        }

        public async Task<Question?> GetByIdAsync(int questionId)
        {
            try
            {
                return await _db.Questions
                    .Include(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(q => q.QuestionId == questionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve question with QuestionId {QuestionId}.",
                    questionId);

                throw;
            }
        }

        public async Task<bool> CreateAsync(Question question)
        {
            try
            {
                _db.Questions.Add(question);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create question.");

                return false;
            }
        }

        public async Task<bool> UpdateAsync(Question question)
        {
            try
            {
                var existingQuestion = await _db.Questions
                    .Include(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(
                        q => q.QuestionId == question.QuestionId);

                if (existingQuestion == null)
                {
                    _logger.LogWarning(
                        "Cannot update question. QuestionId {QuestionId} was not found.",
                        question.QuestionId);

                    return false;
                }

                // Protects recorded answers by blocking changes to used questions.
                bool hasRecordedAnswers = await _db.AttemptAnswers
                    .AnyAsync(a => a.QuestionId == question.QuestionId);

                if (hasRecordedAnswers)
                {
                    _logger.LogWarning(
                        "Cannot update question {QuestionId}: " +
                        "the question is used in recorded answers.",
                        question.QuestionId);

                    return false;
                }

                var incomingOptions = question.AnswerOptions.ToList();

                var existingOptions = existingQuestion.AnswerOptions.ToList();

                var incomingIds = incomingOptions
                    .Where(a => a.AnswerOptionId != 0)
                    .Select(a => a.AnswerOptionId)
                    .ToList();

                // Rejects duplicate identifiers and options from other questions.
                bool hasDuplicateIds =
                    incomingIds.Count != incomingIds.Distinct().Count();

                bool hasUnknownIds = incomingIds.Any(id =>
                    !existingOptions.Any(a => a.AnswerOptionId == id));

                if (hasDuplicateIds || hasUnknownIds)
                {
                    _logger.LogWarning(
                        "Cannot update question {QuestionId}: invalid answer option identifiers.",
                        question.QuestionId);

                    return false;
                }

                existingQuestion.Text = question.Text;
                existingQuestion.Explanation = question.Explanation;
                existingQuestion.ImageUrl = question.ImageUrl;
                existingQuestion.AllowMultipleAnswers =
                    question.AllowMultipleAnswers;

                // Removes options omitted from the submitted complete list.
                foreach (var existingOption in existingOptions)
                {
                    bool keepOption = incomingIds.Contains(
                        existingOption.AnswerOptionId);

                    if (!keepOption)
                    {
                        _db.AnswerOptions.Remove(existingOption);
                    }
                }

                foreach (var incomingOption in incomingOptions)
                {
                    if (incomingOption.AnswerOptionId == 0)
                    {
                        // Creates an option belonging to this question.
                        existingQuestion.AnswerOptions.Add(new AnswerOption
                        {
                            Text = incomingOption.Text,
                            ImageUrl = incomingOption.ImageUrl,
                            IsCorrect = incomingOption.IsCorrect
                        });
                    }
                    else
                    {
                        // Updates an option already belonging to this question.
                        var existingOption = existingOptions.First(a =>
                            a.AnswerOptionId ==
                            incomingOption.AnswerOptionId);

                        existingOption.Text = incomingOption.Text;
                        existingOption.ImageUrl = incomingOption.ImageUrl;
                        existingOption.IsCorrect = incomingOption.IsCorrect;
                    }
                }

                // Saves the question and option changes together.
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update question with QuestionId {QuestionId}.",
                    question.QuestionId);

                return false;
            }
        }

        public async Task<bool> DeleteAsync(int questionId)
        {
            try
            {
                var question = await _db.Questions
                    .FirstOrDefaultAsync(
                        q => q.QuestionId == questionId);

                if (question == null)
                {
                    _logger.LogWarning(
                        "Cannot delete question. QuestionId {QuestionId} was not found.",
                        questionId);

                    return false;
                }

                _db.Questions.Remove(question);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete question with QuestionId {QuestionId}.",
                    questionId);

                return false;
            }
        }
    }
}