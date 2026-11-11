using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger<QuizController> _logger;

        public QuizController(
            IQuizRepository quizRepository,
            ILogger<QuizController> logger)
        {
            _quizRepository = quizRepository;
            _logger = logger;
        }

        // Displays all available quizzes.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var quizzes = await _quizRepository.GetAll();

            if (quizzes == null)
            {
                _logger.LogError(
                    "[QuizController] Failed to retrieve the quiz list.");

                return StatusCode(
                    500,
                    "The quizzes could not be loaded. Please try again.");
            }

            var viewModel = quizzes.Select(quiz =>
                new QuizListItemViewModel
                {
                    QuizId = quiz.QuizId,
                    PageTitle = quiz.Title,
                    Description = quiz.Description
                }).ToList();

            return View(viewModel);
        }

        // Displays information about the selected quiz.
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var quiz = await _quizRepository.GetQuizById(id);

            if (quiz == null)
            {
                _logger.LogWarning(
                    "[QuizController] Quiz was not available for QuizId {QuizId}.",
                    id);

                return NotFound("The quiz was not found.");
            }

            var viewModel = new QuizDetailsViewModel
            {
                QuizId = quiz.QuizId,
                PageTitle = quiz.Title,
                Description = quiz.Description,
                QuestionCount = quiz.Questions.Count
            };

            return View(viewModel);
        }

        // Displays the quiz questions to an authenticated user.
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Play(int id)
        {
            var quiz = await _quizRepository.GetQuizById(id);

            if (quiz == null)
            {
                _logger.LogWarning(
                    "[QuizController] Quiz was not available when starting QuizId {QuizId}.",
                    id);

                return NotFound("The quiz was not found.");
            }

            var viewModel = new QuizPlayViewModel
            {
                QuizId = quiz.QuizId,
                PageTitle = quiz.Title,

                Questions = quiz.Questions
                    .Select((question, index) => new QuestionViewModel
                    {
                        QuestionId = question.QuestionId,
                        QuestionText = question.QuestionText,
                        ImageUrl = question.ImageUrl,
                        QuestionIndex = index,

                        IsMultipleChoice =
                            question.QuestionType == QuestionType.TextMultiple ||
                            question.QuestionType == QuestionType.ImageMultiple,

                        IsImageQuestion =
                            question.QuestionType == QuestionType.ImageSingle ||
                            question.QuestionType == QuestionType.ImageMultiple,

                        Options = question.AnswerOptions
                            .Select(option => new AnswerOptionViewModel
                            {
                                Id = option.AnswerOptionId,
                                Text = option.AnswerText,
                                ImageUrl = option.ImageUrl
                            }).ToList()
                    }).ToList()
            };

            return View(viewModel);
        }
    }
}