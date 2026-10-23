// The DAL folder contains database configuration and data access code.
// This interface defines database operations for quizzes and question links.

using QuizApi.Models;

namespace QuizApi.DAL
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllAsync();

        Task<Quiz?> GetByIdAsync(int quizId);

        Task<bool> CreateAsync(Quiz quiz);

        Task<bool> UpdateAsync(Quiz quiz);

        Task<bool> DeleteAsync(int quizId);

        // Links an existing question to a quiz at the specified position.
        Task<bool> AddQuestionAsync(
            int quizId, int questionId, int order);

        // Updates the position of a question within a quiz.
        Task<bool> UpdateQuestionOrderAsync(
            int quizId, int questionId, int order);

        // Removes the link without deleting the question itself.
        Task<bool> RemoveQuestionAsync(
            int quizId, int questionId);
    }
}