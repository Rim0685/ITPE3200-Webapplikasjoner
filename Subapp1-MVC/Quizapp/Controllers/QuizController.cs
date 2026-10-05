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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var quizzes = await _quizRepository.GetAll();

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

                        QuestionIndex = index,
                        
                        IsMultipleChoice =
                            question.QuestionType == QuestionType.TextMultiple ||
                            question.QuestionType == QuestionType.ImageMultiple,

                        IsImageQuestion =
                            question.QuestionType == QuestionType.ImageSingle ||
                            question.QuestionType == QuestionType.ImageMultiple,

                        
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