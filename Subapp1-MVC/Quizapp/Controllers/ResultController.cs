// The Controllers folder contains classes that handle web requests.
// This controller saves quiz attempts and controls access to results and solutions.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers
{
    [Authorize]
    public class ResultController : Controller
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger<ResultController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        // Receives repository, logging and user management services.
        public ResultController(
            IQuizRepository quizRepository,
            ILogger<ResultController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _quizRepository = quizRepository;
            _logger = logger;
            _userManager = userManager;
        }

        // Processes submitted answers and saves the user's completed attempt.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(
            SubmitQuizViewModel submission)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Challenge();
                }

                if (submission.Answers == null
                    || !submission.Answers.Any())
                {
                    _logger.LogWarning(
                        "Submission for QuizId {QuizId} contained no answers.",
                        submission.QuizId);

                    return View("SubmissionError");
                }

                var quiz = await _quizRepository.GetQuizById(
                    submission.QuizId);

                if (quiz == null)
                {
                    _logger.LogWarning(
                        "QuizId {QuizId} was not found during submission.",
                        submission.QuizId);

                    return NotFound("The quiz was not found.");
                }

                // Associates the completed attempt with its owner.
                var attempt = new QuizAttempt
                {
                    UserId = user.Id,
                    QuizId = submission.QuizId,
                    TotalQuestions = quiz.Questions.Count,
                    CompletedAt = DateTime.UtcNow
                };

                int score = 0;

                foreach (var answer in submission.Answers)
                {
                    var question = quiz.Questions
                        .FirstOrDefault(q =>
                            q.QuestionId == answer.QuestionId);

                    if (question == null)
                    {
                        _logger.LogWarning(
                            "QuestionId {QuestionId} was not found in QuizId {QuizId}.",
                            answer.QuestionId,
                            submission.QuizId);

                        continue;
                    }

                    var selectedIds = answer.SelectedAnswerOptionIds
                        .OrderBy(id => id)
                        .ToList();

                    // Rejects options that do not belong to the question.
                    bool hasInvalidOptions = selectedIds.Any(id =>
                        !question.AnswerOptions.Any(option =>
                            option.AnswerOptionId == id));

                    if (hasInvalidOptions)
                    {
                        _logger.LogWarning(
                            "Invalid answer options submitted for QuestionId {QuestionId}.",
                            question.QuestionId);

                        return View("SubmissionError");
                    }

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

                // EF Core assigns the attempt ID when the attempt is saved.
                return RedirectToAction(
                    nameof(Result),
                    new { id = attempt.QuizAttemptId });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process submission for QuizId {QuizId}.",
                    submission.QuizId);

                return View("SubmissionError");
            }
        }

        // Displays the result and calculates whether the solution is unlocked.
        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var attempt = await _quizRepository.GetQuizAttemptById(id);

                if (attempt == null || attempt.UserId != userId)
                {
                    return NotFound();
                }

                // Retrieves only this user's saved attempts for the same quiz.
                var userAttempts =
                    await _quizRepository.GetUserQuizAttempts(userId);

                var quizAttempts = userAttempts
                    .Where(savedAttempt =>
                        savedAttempt.QuizId == attempt.QuizId)
                    .ToList();

                bool hasTwoAttempts = quizAttempts.Count >= 2;

                bool hasPerfectFirstAttempt =
                    quizAttempts.Count == 1
                    && quizAttempts[0].TotalQuestions > 0
                    && quizAttempts[0].Score
                        == quizAttempts[0].TotalQuestions;

                var viewModel = new ResultViewModel
                {
                    QuizAttemptId = attempt.QuizAttemptId,
                    QuizTitle = attempt.Quiz?.Title ?? "Quiz",
                    Score = attempt.Score,
                    TotalQuestions = attempt.TotalQuestions,
                    AttemptCount = quizAttempts.Count,
                    SolutionUnlocked =
                        hasTwoAttempts || hasPerfectFirstAttempt
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to load quiz result for QuizAttemptId {QuizAttemptId}.",
                    id);

                return StatusCode(
                    500,
                    "The result could not be loaded. Please try again.");
            }
        }

        // Displays the solution only when the user's attempts unlock it.
        [HttpGet]
        public async Task<IActionResult> Solution(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var attempt = await _quizRepository.GetQuizAttemptById(id);

                // Only the owner can request the solution.
                if (attempt == null || attempt.UserId != userId)
                {
                    return NotFound();
                }

                var userAttempts =
                    await _quizRepository.GetUserQuizAttempts(userId);

                var quizAttempts = userAttempts
                    .Where(savedAttempt =>
                        savedAttempt.QuizId == attempt.QuizId)
                    .ToList();

                bool hasTwoAttempts = quizAttempts.Count >= 2;

                bool hasPerfectFirstAttempt =
                    quizAttempts.Count == 1
                    && quizAttempts[0].TotalQuestions > 0
                    && quizAttempts[0].Score
                        == quizAttempts[0].TotalQuestions;

                bool solutionUnlocked =
                    hasTwoAttempts || hasPerfectFirstAttempt;

                if (!solutionUnlocked)
                {
                    _logger.LogWarning(
                        "Solution access denied for QuizAttemptId {QuizAttemptId}: insufficient attempts or score.",
                        id);

                    TempData["ErrorMessage"] =
                        "The solution unlocks after two completed attempts " +
                        "or a perfect score on your first attempt.";

                    return RedirectToAction(
                        nameof(Result), new { id });
                }

                // Loads all questions, including those the user did not answer.
var quiz = await _quizRepository.GetQuizById(attempt.QuizId);

if (quiz == null)
{
    return NotFound("The quiz was not found.");
}

var questionResults = new List<QuestionResultViewModel>();

foreach (var question in quiz.Questions)
{
    // Retrieves the selected options for this question.
    var selectedAnswers = attempt.AttemptAnswers
        .Where(answer =>
            answer.AnswerOption != null
            && answer.AnswerOption.QuestionId == question.QuestionId)
        .ToList();

    var selectedIds = selectedAnswers
        .Select(answer => answer.AnswerOptionId)
        .OrderBy(optionId => optionId)
        .ToList();

    var correctOptions = question.AnswerOptions
        .Where(option => option.IsCorrect)
        .ToList();

    var correctIds = correctOptions
        .Select(option => option.AnswerOptionId)
        .OrderBy(optionId => optionId)
        .ToList();

    // Shows one result per question and includes every correct option.
    questionResults.Add(new QuestionResultViewModel
    {
        QuestionText = question.QuestionText,

        SelectedAnswerText = selectedAnswers.Count == 0
            ? "Not answered"
            : string.Join("; ", selectedAnswers.Select(answer =>
                answer.AnswerOption!.AnswerText
                ?? answer.AnswerOption.ImageUrl
                ?? "Image option")),

        CorrectAnswerText = string.Join("; ",
            correctOptions.Select(option =>
                option.AnswerText
                ?? option.ImageUrl
                ?? "Image option")),

        // Uses the same exact-match rule as the score calculation.
        IsCorrect = correctIds.Count > 0
            && selectedIds.SequenceEqual(correctIds)
    });
}

                var viewModel = new SolutionViewModel
                {
                    QuizAttemptId = attempt.QuizAttemptId,
                    Questions = questionResults
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to load solution for QuizAttemptId {QuizAttemptId}.",
                    id);

                return StatusCode(
                    500,
                    "The solution could not be loaded. Please try again.");
            }
        }
    }
}