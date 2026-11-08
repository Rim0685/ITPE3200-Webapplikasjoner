using Quizapp.Models;

namespace Quizapp.DAL
{
    // Defines the database operations that can be performed for quizzes.
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAll();
        Task<Quiz?> GetQuizById(int id);
        Task Create(Quiz quiz);
        Task Update(Quiz quiz);
        Task<bool> Delete(int id);

        Task AddQuizAttempt(QuizAttempt quizAttempt);
        Task<QuizAttempt?> GetQuizAttemptById(int id);
        Task<QuizAttempt?> GetLatestQuizAttempt(int quizId);
        Task<int> GetQuizAttemptCount(int quizId);

        // Retrieves all completed quiz attempts belonging to a user.
        Task<IEnumerable<QuizAttempt>> GetUserQuizAttempts(string userId);

        // Deletes a quiz attempt only if it belongs to the specified user.
        Task<bool> DeleteUserQuizAttempt(int attemptId, string userId);
    }
}