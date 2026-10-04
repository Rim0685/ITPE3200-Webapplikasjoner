using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers
{
    public class ResultController : Controller
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger<ResultController> _logger;

        public ResultController(IQuizRepository quizRepository, ILogger<ResultController> logger)
        {
            _quizRepository = quizRepository;
            _logger = logger;
        }

        // POST: kalles når eleven sender inn svarene sine på quizen
        [HttpPost]
        public async Task<IActionResult> Submit(SubmitQuizViewModel submission)
        {
            if (submission.Answers == null || !submission.Answers.Any())
            {
                _logger.LogWarning("[ResultController] Quiz-innsending for QuizId {QuizId} inneholdt ingen svar.", submission.QuizId);
                return View("SubmissionError");
            }

            var quiz = await _quizRepository.GetQuizById(submission.QuizId);
            if (quiz == null)
            {
                _logger.LogError("[ResultController] Fant ikke quiz for QuizId {QuizId} ved innsending.", submission.QuizId);
                return NotFound("Quiz not found");
            }

            try
            {
                // Bygger en oppslagstabell over alle svaralternativer som hører til denne quizen,
                // siden repositoriet ikke har en direkte GetAnswerOptionById-metode.
                var allOptions = quiz.Questions
                    .SelectMany(q => q.AnswerOptions)
                    .ToDictionary(o => o.AnswerOptionId);

                var attempt = new QuizAttempt
                {
                    QuizId = submission.QuizId,
                    TotalQuestions = submission.Answers.Count,
                    CompletedAt = DateTime.UtcNow
                };

                int score = 0;

                foreach (var answer in submission.Answers)
                {
                    if (!allOptions.TryGetValue(answer.SelectedAnswerOptionId, out var selectedOption))
                    {
                        _logger.LogWarning("[ResultController] Fant ikke AnswerOptionId {Id} i QuizId {QuizId}.", answer.SelectedAnswerOptionId, submission.QuizId);
                        continue;
                    }

                    if (selectedOption.IsCorrect)
                        score++;

                    attempt.AttemptAnswers.Add(new AttemptAnswer
                    {
                        AnswerOptionId = selectedOption.AnswerOptionId
                    });
                }

                attempt.Score = score;

                await _quizRepository.AddQuizAttempt(attempt);
                // attempt.QuizAttemptId fylles ut av EF Core etter SaveChangesAsync,
                // siden 'attempt' er den samme sporede entiteten.

                return RedirectToAction(nameof(Result), new { id = attempt.QuizAttemptId });
            }
            catch (Exception e)
            {
                _logger.LogError("[ResultController] Feil ved poengberegning for QuizId {QuizId}, feilmelding: {e}", submission.QuizId, e.Message);
                return View("SubmissionError");
            }
        }

        // GET: viser oppsummeringen av resultatet
        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var attempt = await _quizRepository.GetQuizAttemptById(id);

            if (attempt == null)
            {
                _logger.LogError("[ResultController] Fant ikke QuizAttempt for QuizAttemptId {QuizAttemptId}", id);
                return NotFound("Quiz attempt not found");
            }

            var viewModel = new ResultViewModel
            {
                QuizAttemptId = attempt.QuizAttemptId,
                QuizTitle = attempt.Quiz?.Title ?? "Quiz",
                Score = attempt.Score,
                TotalQuestions = attempt.TotalQuestions
            };

            return View(viewModel);
        }

        // GET: viser full oversikt med riktige svar
        [HttpGet]
        public async Task<IActionResult> Solution(int id)
        {
            var attempt = await _quizRepository.GetQuizAttemptById(id);

            if (attempt == null)
            {
                _logger.LogError("[ResultController] Fant ikke QuizAttempt for QuizAttemptId {QuizAttemptId}", id);
                return NotFound("Quiz attempt not found");
            }

            var questionResults = attempt.AttemptAnswers
                .Where(aa => aa.AnswerOption?.Question != null)
                .Select(aa => new QuestionResultViewModel
                {
                    QuestionText = aa.AnswerOption!.Question!.QuestionText,
                    SelectedAnswerText = aa.AnswerOption.AnswerText,
                    IsCorrect = aa.AnswerOption.IsCorrect,
                    CorrectAnswerText = aa.AnswerOption.Question.AnswerOptions
                        .FirstOrDefault(o => o.IsCorrect)?.AnswerText
                })
                .ToList();

            var viewModel = new SolutionViewModel
            {
                QuizAttemptId = attempt.QuizAttemptId,
                Questions = questionResults
            };

            return View(viewModel);
        }
    }
}