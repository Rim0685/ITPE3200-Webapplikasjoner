using QuizApi.DTOs;

namespace QuizApi.Services
{
    public interface IQuizEvaluationService
    {
        Task<QuizResultDto?> EvaluateAndSaveAsync(SubmitQuizDto submission, string userId);
        Task<QuizSolutionDto?> GetSolutionAsync(int quizAttemptId, string userId);
    }
}