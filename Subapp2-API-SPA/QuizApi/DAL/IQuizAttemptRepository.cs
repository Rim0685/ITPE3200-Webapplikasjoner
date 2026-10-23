// The DAL folder contains database configuration and data access code.
// This interface defines database operations for completed quiz attempts.

using QuizApi.Models;

namespace QuizApi.DAL
{
    public interface IQuizAttemptRepository
    {
        Task<bool> CreateAsync(QuizAttempt attempt);

        Task<QuizAttempt?> GetByIdAsync(
            int quizAttemptId, string userId);

        Task<QuizAttempt?> GetLatestAsync(
            int quizId, string userId);

        Task<int> CountAsync(
            int quizId, string userId);
    }
}