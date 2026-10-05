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

        // POST: Called when the student submits their answers to the quiz.
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
                var attempt = new QuizAttempt
                {
                    QuizId = submission.QuizId,
                    TotalQuestions = quiz.Questions.Count,
                    CompletedAt = DateTime.UtcNow
                };

                int score = 0;

                foreach (var answer in submission.Answers)
                {
                    var question = quiz.Questions
                        .FirstOrDefault(q => q.QuestionId == answer.QuestionId);

                    if (question == null)
                    {
                        _logger.LogWarning(
                            "[ResultController] Fant ikke QuestionId {QuestionId} i QuizId {QuizId}.",
                            answer.QuestionId,
                            submission.QuizId);

                        continue;    
                    }

                    var selectedIds = answer.SelectedAnswerOptionIds
                        .OrderBy(id => id)
                        .ToList();

                    var correctIds = question.AnswerOptions
                        .Where(option => option.IsCorrect)
                        .Select(option => option.AnswerOptionId)
                        .OrderBy(id => id)
                        .ToList();

                    if (selectedIds.SequenceEqual(correctIds))
                    {
                        score++;
                    }        

                    foreach (var selectedId in selectedIds)
                    {
                        attempt.AttemptAnswers.Add(new AttemptAnswer
                        {
                            AnswerOptionId = selectedId
                        });
                    }
                }

                attempt.Score = score;

                await _quizRepository.AddQuizAttempt(attempt);
                // attempt.QuizAttemptId is populated by EF Core after SaveChangesAsync,
                // because 'attempt' is the same tracked entity.

                return RedirectToAction(nameof(Result), new { id = attempt.QuizAttemptId });
            }
            catch (Exception e)
            {
                _logger.LogError("[ResultController] Feil ved poengberegning for QuizId {QuizId}, feilmelding: {e}", submission.QuizId, e.Message);
                return View("SubmissionError");
            }
        }

        // GET: Displays a summary of the quiz result.
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

        // GET: Displays a full overview with correct answer.
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