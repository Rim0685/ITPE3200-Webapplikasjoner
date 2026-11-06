using QuizApi.DAL;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Services
{
    // Scoring rule: each question is worth 1 point total. If a question has
    // N correct options, each correct one selected is worth 1/N points.
    // Selecting ANY wrong option zeroes the whole question. Unanswered = 0
    //
    // Fasit unlock rule: unlocked after 2 completed attempts, OR unlocked
    // immediately if the first attempt scores full marks
    public class QuizEvaluationService : IQuizEvaluationService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizAttemptRepository _quizAttemptRepository;
        private readonly ILogger<QuizEvaluationService> _logger;

        public QuizEvaluationService(
            IQuizRepository quizRepository,
            IQuizAttemptRepository quizAttemptRepository,
            ILogger<QuizEvaluationService> logger)
        {
            _quizRepository = quizRepository;
            _quizAttemptRepository = quizAttemptRepository;
            _logger = logger;
        }

        public async Task<QuizResultDto?> EvaluateAndSaveAsync(SubmitQuizDto submission, string userId)
        {
            var quiz = await _quizRepository.GetByIdAsync(submission.QuizId);
            if (quiz == null)
            {
                _logger.LogWarning("[QuizEvaluationService] Quiz {QuizId} not found during submission.", submission.QuizId);
                return null;
            }

            try
            {
                var attempt = new QuizAttempt
                {
                    QuizId = submission.QuizId,
                    UserId = userId,
                    CompletedAt = DateTime.UtcNow
                };

                decimal totalScore = 0;
                decimal maxScore = quiz.QuizQuestions.Count;
                var questionResults = new List<QuestionResultDto>();

                foreach (var quizQuestion in quiz.QuizQuestions)
                {
                    var question = quizQuestion.Question;
                    if (question == null) continue;

                    var submittedAnswer = submission.Answers
                        .FirstOrDefault(a => a.QuestionId == question.QuestionId);
                    var selectedIds = submittedAnswer?.SelectedAnswerOptionIds ?? new List<int>();

                    var correctOptions = question.AnswerOptions.Where(o => o.IsCorrect).ToList();
                    var selectedOptions = question.AnswerOptions
                        .Where(o => selectedIds.Contains(o.AnswerOptionId))
                        .ToList();

                    bool wrongSelected = selectedOptions.Any(o => !o.IsCorrect);
                    int correctCount = correctOptions.Count;
                    int correctSelectedCount = selectedOptions.Count(o => o.IsCorrect);

                    decimal awarded = 0;
                    if (!wrongSelected && correctCount > 0)
                    {
                        decimal perOption = 1m / correctCount;
                        awarded = perOption * correctSelectedCount;
                    }

                    bool wasFullyCorrect = !wrongSelected
                        && correctCount > 0
                        && correctSelectedCount == correctCount;

                    totalScore += awarded;

                    // Stub entities (ID only) so EF Core attaches these as
                    // existing rows instead of inserting duplicates
                    var selectedStubs = selectedOptions
                        .Select(o => new AnswerOption { AnswerOptionId = o.AnswerOptionId })
                        .ToList();

                    attempt.AttemptAnswers.Add(new AttemptAnswer
                    {
                        QuestionId = question.QuestionId,
                        SelectedOptions = selectedStubs,
                        AwardedPoints = awarded
                    });

                    questionResults.Add(new QuestionResultDto
                    {
                        QuestionId = question.QuestionId,
                        WasFullyCorrect = wasFullyCorrect,
                        AwardedPoints = awarded
                    });
                }

                attempt.Score = totalScore;
                attempt.MaxScore = maxScore;

                bool saved = await _quizAttemptRepository.CreateAsync(attempt);
                if (!saved)
                {
                    _logger.LogError("[QuizEvaluationService] Failed to save QuizAttempt for QuizId {QuizId}, UserId {UserId}.", submission.QuizId, userId);
                    return null;
                }

                int attemptCount = await _quizAttemptRepository.CountAsync(submission.QuizId, userId);
                bool perfectFirstAttempt = attemptCount == 1 && totalScore == maxScore;
                bool unlocked = attemptCount >= 2 || perfectFirstAttempt;

                return new QuizResultDto
                {
                    QuizAttemptId = attempt.QuizAttemptId,
                    Score = totalScore,
                    MaxScore = maxScore,
                    AttemptCount = attemptCount,
                    IsSolutionUnlocked = unlocked,
                    QuestionResults = questionResults
                };
            }
            catch (Exception e)
            {
                _logger.LogError("[QuizEvaluationService] Error evaluating QuizId {QuizId} for UserId {UserId}, error: {Error}", submission.QuizId, userId, e.Message);
                return null;
            }
        }

        public async Task<QuizSolutionDto?> GetSolutionAsync(int quizAttemptId, string userId)
        {
            var attempt = await _quizAttemptRepository.GetByIdAsync(quizAttemptId, userId);
            if (attempt == null)
            {
                _logger.LogWarning("[QuizEvaluationService] QuizAttempt {Id} not found for UserId {UserId}.", quizAttemptId, userId);
                return null;
            }

            int attemptCount = await _quizAttemptRepository.CountAsync(attempt.QuizId, userId);
            bool perfectFirstAttempt = attemptCount == 1 && attempt.Score == attempt.MaxScore;
            bool unlocked = attemptCount >= 2 || perfectFirstAttempt;

            if (!unlocked)
            {
                _logger.LogWarning("[QuizEvaluationService] Solution requested but not yet unlocked for UserId {UserId}, QuizAttempt {Id}.", userId, quizAttemptId);
                return null;
            }

            var questions = attempt.AttemptAnswers.Select(aa => new QuestionSolutionDto
            {
                QuestionId = aa.QuestionId,
                Text = aa.Question?.Text ?? string.Empty,
                Explanation = aa.Question?.Explanation ?? string.Empty,
                SelectedAnswerOptionIds = aa.SelectedOptions.Select(o => o.AnswerOptionId).ToList(),
                CorrectAnswerOptionIds = aa.Question?.AnswerOptions
                    .Where(o => o.IsCorrect)
                    .Select(o => o.AnswerOptionId)
                    .ToList() ?? new List<int>(),
                WasFullyCorrect = aa.AwardedPoints == 1
            }).ToList();

            return new QuizSolutionDto
            {
                QuizAttemptId = attempt.QuizAttemptId,
                Questions = questions
            };
        }
    }
}