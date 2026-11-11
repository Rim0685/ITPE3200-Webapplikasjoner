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

        public ResultController(
            IQuizRepository quizRepository,
            ILogger<ResultController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _quizRepository = quizRepository;
            _logger = logger;
            _userManager = userManager;
        }

        // Validates submitted answers and saves the completed attempt.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(
            SubmitQuizViewModel submission)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (!ModelState.IsValid
                || submission.Answers == null
                || !submission.Answers.Any())
            {
                _logger.LogWarning(
                    "[ResultController] Invalid submission for QuizId {QuizId}.",
                    submission.QuizId);

                return View("SubmissionError");
            }

            var quiz = await _quizRepository.GetQuizById(
                submission.QuizId);

            if (quiz == null)
            {
                _logger.LogWarning(
                    "[ResultController] Quiz was not available for QuizId {QuizId}.",
                    submission.QuizId);

                return NotFound("The quiz was not found.");
            }

            if (!quiz.Questions.Any())
            {
                _logger.LogWarning(
                    "[ResultController] QuizId {QuizId} contained no questions.",
                    quiz.QuizId);

                return View("SubmissionError");
            }

            // Each question may appear only once in a submission.
            bool hasDuplicateQuestions = submission.Answers
                .Select(answer => answer.QuestionId)
                .Distinct()
                .Count() != submission.Answers.Count;

            if (hasDuplicateQuestions)
            {
                _logger.LogWarning(
                    "[ResultController] Duplicate questions submitted for QuizId {QuizId}.",
                    quiz.QuizId);

                return View("SubmissionError");
            }

            // Sets ownership and result information on the server.
            var attempt = new QuizAttempt
            {
                UserId = userId,
                QuizId = quiz.QuizId,
                TotalQuestions = quiz.Questions.Count,
                CompletedAt = DateTime.UtcNow
            };

            int score = 0;

            foreach (var answer in submission.Answers)
            {
                var question = quiz.Questions
                    .FirstOrDefault(question =>
                        question.QuestionId == answer.QuestionId);

                if (question == null)
                {
                    _logger.LogWarning(
                        "[ResultController] QuestionId {QuestionId} does not belong to QuizId {QuizId}.",
                        answer.QuestionId,
                        quiz.QuizId);

                    return View("SubmissionError");
                }

                if (answer.SelectedAnswerOptionIds == null)
                {
                    _logger.LogWarning(
                        "[ResultController] Missing answer option list for QuestionId {QuestionId}.",
                        question.QuestionId);

                    return View("SubmissionError");
                }

                // Each selected option may appear only once.
                bool hasDuplicateOptions = answer.SelectedAnswerOptionIds
                    .Distinct()
                    .Count() != answer.SelectedAnswerOptionIds.Count;

                if (hasDuplicateOptions)
                {
                    _logger.LogWarning(
                        "[ResultController] Duplicate options submitted for QuestionId {QuestionId}.",
                        question.QuestionId);

                    return View("SubmissionError");
                }

                var selectedIds = answer.SelectedAnswerOptionIds
                    .OrderBy(id => id)
                    .ToList();

                bool isMultipleChoice =
                    question.QuestionType == QuestionType.TextMultiple
                    || question.QuestionType == QuestionType.ImageMultiple;

                // Single-choice questions allow at most one selected option.
                if (!isMultipleChoice && selectedIds.Count > 1)
                {
                    _logger.LogWarning(
                        "[ResultController] Multiple options submitted for single-choice QuestionId {QuestionId}.",
                        question.QuestionId);

                    return View("SubmissionError");
                }

                // Every selected option must belong to this question.
                bool hasInvalidOptions = selectedIds.Any(id =>
                    !question.AnswerOptions.Any(option =>
                        option.AnswerOptionId == id));

                if (hasInvalidOptions)
                {
                    _logger.LogWarning(
                        "[ResultController] Invalid options submitted for QuestionId {QuestionId}.",
                        question.QuestionId);

                    return View("SubmissionError");
                }

                var correctIds = question.AnswerOptions
                    .Where(option => option.IsCorrect)
                    .Select(option => option.AnswerOptionId)
                    .OrderBy(id => id)
                    .ToList();

                // Awards one point only for an exact match with the correct options.
                if (correctIds.Count > 0
                    && selectedIds.SequenceEqual(correctIds))
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

            // Checks that the attempt was saved before displaying the result.
            bool returnOk = await _quizRepository.AddQuizAttempt(attempt);

            if (!returnOk)
            {
                _logger.LogError(
                    "[ResultController] Failed to save an attempt for QuizId {QuizId}.",
                    quiz.QuizId);

                return View("SubmissionError");
            }

            return RedirectToAction(
                nameof(Result),
                new { id = attempt.QuizAttemptId });
        }

        // Displays the owner's result and solution access status.
        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var attempt = await _quizRepository.GetQuizAttemptById(id);

            if (attempt == null || attempt.UserId != userId)
            {
                _logger.LogWarning(
                    "[ResultController] QuizAttemptId {QuizAttemptId} was not available to the user.",
                    id);

                return NotFound();
            }

            var userAttempts =
                await _quizRepository.GetUserQuizAttempts(userId);

            if (userAttempts == null)
            {
                _logger.LogError(
                    "[ResultController] Failed to retrieve the user's quiz attempts.");

                return StatusCode(
                    500,
                    "The result could not be loaded. Please try again.");
            }

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

        // Displays the solution only when the owner's attempts unlock it.
        [HttpGet]
        public async Task<IActionResult> Solution(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var attempt = await _quizRepository.GetQuizAttemptById(id);

            if (attempt == null || attempt.UserId != userId)
            {
                _logger.LogWarning(
                    "[ResultController] QuizAttemptId {QuizAttemptId} was not available to the user.",
                    id);

                return NotFound();
            }

            var userAttempts =
                await _quizRepository.GetUserQuizAttempts(userId);

            if (userAttempts == null)
            {
                _logger.LogError(
                    "[ResultController] Failed to retrieve the user's quiz attempts.");

                return StatusCode(
                    500,
                    "The solution could not be loaded. Please try again.");
            }

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
                    "[ResultController] Solution access denied for QuizAttemptId {QuizAttemptId}.",
                    id);

                TempData["ErrorMessage"] =
                    "The solution unlocks after two completed attempts " +
                    "or a perfect score on your first attempt.";

                return RedirectToAction(
                    nameof(Result),
                    new { id });
            }

            var quiz = await _quizRepository.GetQuizById(attempt.QuizId);

            if (quiz == null)
            {
                _logger.LogWarning(
                    "[ResultController] Quiz was not available for QuizId {QuizId}.",
                    attempt.QuizId);

                return NotFound("The quiz was not found.");
            }

            var questionResults = new List<QuestionResultViewModel>();

            // Includes every question, even if the user did not answer it.
            foreach (var question in quiz.Questions)
            {
                var selectedAnswers = attempt.AttemptAnswers
                    .Where(answer =>
                        answer.AnswerOption != null
                        && answer.AnswerOption.QuestionId
                            == question.QuestionId)
                    .ToList();

                var selectedIds = selectedAnswers
                    .Select(answer => answer.AnswerOptionId)
                    .OrderBy(id => id)
                    .ToList();

                var correctOptions = question.AnswerOptions
                    .Where(option => option.IsCorrect)
                    .ToList();

                var correctIds = correctOptions
                    .Select(option => option.AnswerOptionId)
                    .OrderBy(id => id)
                    .ToList();

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
    }
}