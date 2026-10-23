// The DAL folder contains database configuration and data access code.
// This class handles database operations for quizzes and question links.

using Microsoft.EntityFrameworkCore;
using QuizApi.Models;

namespace QuizApi.DAL
{
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

        // Retrieves all quizzes and their question links.
        public async Task<IEnumerable<Quiz>> GetAllAsync()
        {
            try
            {
                return await _db.Quizzes
                    .Include(q => q.QuizQuestions)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve quizzes.");
                throw;
            }
        }

        // Retrieves a quiz with its questions and answer options.
        public async Task<Quiz?> GetByIdAsync(int quizId)
        {
            try
            {
                return await _db.Quizzes
                    .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                    .ThenInclude(q => q!.AnswerOptions)
                    .FirstOrDefaultAsync(q => q.QuizId == quizId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve quiz {QuizId}.",
                    quizId);

                throw;
            }
        }

        // Saves a new quiz.
        public async Task<bool> CreateAsync(Quiz quiz)
        {
            try
            {
                _db.Quizzes.Add(quiz);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create quiz with title {Title}.",
                    quiz.Title);

                return false;
            }
        }

        // Updates the quiz's own editable fields.
        public async Task<bool> UpdateAsync(Quiz quiz)
        {
            try
            {
                var existingQuiz = await _db.Quizzes
                    .FirstOrDefaultAsync(q => q.QuizId == quiz.QuizId);

                if (existingQuiz == null)
                {
                    _logger.LogWarning(
                        "Cannot update quiz. QuizId {QuizId} was not found.",
                        quiz.QuizId);

                    return false;
                }

                existingQuiz.Title = quiz.Title;
                existingQuiz.Topic = quiz.Topic;
                existingQuiz.Description = quiz.Description;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update quiz {QuizId}.",
                    quiz.QuizId);

                return false;
            }
        }

        // Deletes a quiz by its identifier.
        public async Task<bool> DeleteAsync(int quizId)
        {
            try
            {
                var quiz = await _db.Quizzes
                    .FirstOrDefaultAsync(q => q.QuizId == quizId);

                if (quiz == null)
                {
                    _logger.LogWarning(
                        "Cannot delete quiz. QuizId {QuizId} was not found.",
                        quizId);

                    return false;
                }

                _db.Quizzes.Remove(quiz);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete quiz {QuizId}.",
                    quizId);

                return false;
            }
        }

        // Links an existing question to an existing quiz.
        public async Task<bool> AddQuestionAsync(
            int quizId, int questionId, int order)
        {
            try
            {
                var quizExists = await _db.Quizzes
                    .AnyAsync(q => q.QuizId == quizId);

                var questionExists = await _db.Questions
                    .AnyAsync(q => q.QuestionId == questionId);

                var linkExists = await _db.QuizQuestions
                    .AnyAsync(qq =>
                        qq.QuizId == quizId &&
                        qq.QuestionId == questionId);

                if (!quizExists || !questionExists || linkExists)
                {
                    _logger.LogWarning(
                        "Cannot link question {QuestionId} to quiz {QuizId}. " +
                        "The quiz or question is missing, or the link already exists.",
                        questionId, quizId);

                    return false;
                }

                var link = new QuizQuestion
                {
                    QuizId = quizId,
                    QuestionId = questionId,
                    Order = order
                };

                _db.QuizQuestions.Add(link);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to link question {QuestionId} to quiz {QuizId}.",
                    questionId, quizId);

                return false;
            }
        }

        // Updates the stored position of a question in a quiz.
        public async Task<bool> UpdateQuestionOrderAsync(
            int quizId, int questionId, int order)
        {
            try
            {
                var link = await _db.QuizQuestions
                    .FirstOrDefaultAsync(qq =>
                        qq.QuizId == quizId &&
                        qq.QuestionId == questionId);

                if (link == null)
                {
                    _logger.LogWarning(
                        "Question link was not found for quiz {QuizId} " +
                        "and question {QuestionId}.",
                        quizId, questionId);

                    return false;
                }

                link.Order = order;
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update question order for quiz {QuizId} " +
                    "and question {QuestionId}.",
                    quizId, questionId);

                return false;
            }
        }

        // Removes the link without deleting the question itself.
        public async Task<bool> RemoveQuestionAsync(
            int quizId, int questionId)
        {
            try
            {
                var link = await _db.QuizQuestions
                    .FirstOrDefaultAsync(qq =>
                        qq.QuizId == quizId &&
                        qq.QuestionId == questionId);

                if (link == null)
                {
                    _logger.LogWarning(
                        "Question link was not found for quiz {QuizId} " +
                        "and question {QuestionId}.",
                        quizId, questionId);

                    return false;
                }

                _db.QuizQuestions.Remove(link);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to remove question {QuestionId} from quiz {QuizId}.",
                    questionId, quizId);

                return false;
            }
        }
    }
}