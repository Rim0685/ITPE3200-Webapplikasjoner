using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers
{
    // Handles the quiz overview, quiz details, and starting a quiz
    public class QuizController : Controller
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger<QuizController> _logger;

        // Dependencies are provided through dependency injection.
        public QuizController(
            IQuizRepository quizRepository,
            ILogger<QuizController> logger)
        {
            _quizRepository = quizRepository;
            _logger = logger;
        }

        // Display all available quizzes.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var quizzes = await _quizRepository.GetAll();

                // Maps database entities to ViewModels used by the view.
                var viewModel = quizzes.Select(quiz => new QuizListItemViewModel
                {
                    QuizId = quiz.QuizId,
                    PageTitle = quiz.Title,
                    Description = quiz.Description
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved henting av quizoversikten.");

                return RedirectToAction("Error", "Home");    
            }
        }

        // Displays information about one selected quiz.
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var quiz = await _quizRepository.GetQuizById(id);

                if (quiz == null)
                {
                    _logger.LogWarning(
                        "Fant ikke quiz med QuizId {QuizId}.",
                        id);

                    return NotFound();    
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
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved henting av quiz med QuizId {QuizId}.",
                    id);

                return RedirectToAction("Error", "Home");    
            }
        }

        // Loads the selected quiz and prepares its questions and answers options.
        [HttpGet]
        public async Task<IActionResult> Play(int id)
        {
            try
            {
                var quiz = await _quizRepository.GetQuizById(id);

                if (quiz == null)
                {
                    _logger.LogWarning(
                        "Fant ikke quiz med QuizId {QuizId} ved oppstart av quiz.",
                        id);

                    return NotFound();    
                }

                var viewModel = new QuizPlayViewModel
                {
                    QuizId = quiz.QuizId,
                    PageTitle = quiz.Title,

                    Questions = quiz.Questions.Select((question, index) => new QuestionViewModel
                    {
                        QuestionId = question.QuestionId,

                        QuestionText = question.QuestionText,

                        ImageUrl = question.ImageUrl,

                        // The index is used when binding submitted answers
                        // To the correct questions in the form.
                        QuestionIndex = index,
                        
                        // Multiple-choice questions allow more than one answer.
                        IsMultipleChoice =
                            question.QuestionType == QuestionType.TextMultiple ||
                            question.QuestionType == QuestionType.ImageMultiple,

                        // Image question display an image as part of the question.
                        IsImageQuestion =
                            question.QuestionType == QuestionType.ImageSingle ||
                            question.QuestionType == QuestionType.ImageMultiple,

                        // Maps answer options from the model to the ViewModel.
                        Options = question.AnswerOptions.Select(option => new AnswerOptionViewModel
                        {
                            Id = option.AnswerOptionId,
                            Text = option.AnswerText,
                            ImageUrl = option.ImageUrl
                        }).ToList()
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved oppstart av quiz med QuizId {QuizId}.",
                    id);

                return RedirectToAction("Error", "Home");
            }
        }
    }
}