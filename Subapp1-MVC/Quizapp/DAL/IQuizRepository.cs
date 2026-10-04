using Quizapp.Models;

namespace Quizapp.DAL
{
    // Bestemmer hvilke databaseoperasjoner som kan utføres for quizer.
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
    }
}