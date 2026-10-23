// The DAL folder contains database configuration and data access code.
// This interface defines database operations for questions and their options.

using QuizApi.Models;

namespace QuizApi.DAL
{
    public interface IQuestionRepository
    {
        // Retrieves all questions with their answer options.
        Task<IEnumerable<Question>> GetAllAsync();

        // Retrieves one question with its answer options.
        Task<Question?> GetByIdAsync(int questionId);

        // Saves a new question with its answer options.
        Task<bool> CreateAsync(Question question);

        // Updates a question and its answer options.
        Task<bool> UpdateAsync(Question question);

        // Deletes a question by its identifier.
        Task<bool> DeleteAsync(int questionId);
    }
}