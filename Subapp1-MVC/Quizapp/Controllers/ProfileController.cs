// Displays and updates the authenticated user's profile.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IQuizRepository _quizRepository;
        private readonly IStudyNoteRepository _studyNoteRepository;
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IQuizRepository quizRepository,
            IStudyNoteRepository studyNoteRepository,
            IFlashcardRepository flashcardRepository,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _quizRepository = quizRepository;
            _studyNoteRepository = studyNoteRepository;
            _flashcardRepository = flashcardRepository;
            _logger = logger;
        }

        // Displays the user's profile and personal content.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Challenge();
                }

                var model = new ProfileViewModel
                {
                    DisplayName = user.DisplayName,
                    FullName = user.FullName,
                    AboutMe = user.AboutMe
                };

                await LoadOverview(model, user.Id);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load the user profile.");

                return StatusCode(
                    500, "The profile could not be loaded. Please try again.");
            }
        }

        // Updates only the editable profile fields.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Challenge();
                }

                if (ModelState.IsValid)
                {
                    user.DisplayName = model.DisplayName.Trim();
                    user.FullName = model.FullName?.Trim();
                    user.AboutMe = model.AboutMe?.Trim();

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation(
                            "The user profile was updated.");

                        TempData["SuccessMessage"] =
                            "Your profile has been updated.";

                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty, error.Description);
                    }

                    _logger.LogWarning("Failed to update the user profile.");
                }

                // Reloads personal content after validation errors.
                await LoadOverview(model, user.Id);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "An error occurred updating the profile.");

                return StatusCode(
                    500, "The profile could not be saved. Please try again.");
            }
        }

        // Deletes a quiz attempt belonging to the authenticated user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuizAttempt(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Challenge();
                }

                bool deleted = await _quizRepository.DeleteUserQuizAttempt(
                    id, user.Id);

                if (!deleted)
                {
                    return NotFound();
                }

                _logger.LogInformation(
                    "Quiz attempt {QuizAttemptId} was deleted.", id);

                TempData["SuccessMessage"] =
                    "The quiz result has been deleted.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Failed to delete quiz attempt {QuizAttemptId}.", id);

                return StatusCode(
                    500, "The quiz result could not be deleted. Please try again.");
            }
        }

        // Loads personal content through the repositories.
        private async Task LoadOverview(
            ProfileViewModel model, string userId)
        {
            var attempts = await _quizRepository.GetUserQuizAttempts(userId);

            model.QuizAttempts = attempts
                .Select(attempt => new QuizAttemptViewModel
                {
                    QuizAttemptId = attempt.QuizAttemptId,
                    QuizTitle = attempt.Quiz?.Title ?? "Quiz",
                    Score = attempt.Score,
                    TotalQuestions = attempt.TotalQuestions,
                    CompletedAt = attempt.CompletedAt
                })
                .ToList();

            var cards = await _flashcardRepository.GetAll(userId);

            model.Flashcards = cards
                .Select(card => new FlashcardSummaryViewModel
                {
                    FlashcardId = card.FlashcardId,
                    QuestionText = card.QuestionText,
                    CreatedAt = card.CreatedAt
                })
                .ToList();

            var notes = await _studyNoteRepository.GetAll(userId);

            model.StudyNotes = notes
                .Select(note => new StudyNoteViewModel
                {
                    StudyNoteId = note.StudyNoteId,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedAt = note.CreatedAt
                })
                .ToList();
        }
    }
}