// The Controllers folder contains classes that handle web requests.
// This controller manages the authenticated user's flashcards.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizapp.DAL;
using Quizapp.Models;

namespace Quizapp.Controllers
{
    [Authorize]
    public class FlashcardController : Controller
    {
        private readonly QuizDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FlashcardController> _logger;

        // Receives database, user management and logging services.
        public FlashcardController(
            QuizDbContext db,
            UserManager<ApplicationUser> userManager,
            ILogger<FlashcardController> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        // Displays the authenticated user's flashcards.
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var flashcards = await _db.Flashcards
                    .Where(card => card.UserId == userId)
                    .OrderByDescending(card => card.CreatedAt)
                    .ToListAsync();

                return View(flashcards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load flashcards.");

                return StatusCode(
                    500,
                    "The flashcards could not be loaded. Please try again.");
            }
        }

        // Displays the form for creating a flashcard.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Creates a flashcard belonging to the authenticated user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("QuestionText,AnswerText")] Flashcard model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                // Sets ownership and creation time on the server.
                var flashcard = new Flashcard
                {
                    QuestionText = model.QuestionText,
                    AnswerText = model.AnswerText,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Flashcards.Add(flashcard);
                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Created flashcard {FlashcardId}.",
                    flashcard.FlashcardId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a flashcard.");

                ModelState.AddModelError(
                    string.Empty,
                    "The flashcard could not be saved. Please try again.");

                return View(model);
            }
        }

        // Displays the edit form for a card owned by the user.
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var flashcard = await _db.Flashcards
                    .FirstOrDefaultAsync(card =>
                        card.FlashcardId == id && card.UserId == userId);

                if (flashcard == null)
                {
                    return NotFound();
                }

                return View(flashcard);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Failed to load flashcard {FlashcardId}.", id);

                return StatusCode(
                    500,
                    "The flashcard could not be loaded. Please try again.");
            }
        }

        // Updates only the editable fields of a card owned by the user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            [Bind("FlashcardId,QuestionText,AnswerText")] Flashcard model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var flashcard = await _db.Flashcards
                    .FirstOrDefaultAsync(card =>
                        card.FlashcardId == model.FlashcardId
                        && card.UserId == userId);

                if (flashcard == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                flashcard.QuestionText = model.QuestionText;
                flashcard.AnswerText = model.AnswerText;

                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Updated flashcard {FlashcardId}.",
                    flashcard.FlashcardId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update flashcard {FlashcardId}.",
                    model.FlashcardId);

                ModelState.AddModelError(
                    string.Empty,
                    "The changes could not be saved. Please try again.");

                return View(model);
            }
        }

        // Displays a deletion confirmation for a card owned by the user.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var flashcard = await _db.Flashcards
                    .FirstOrDefaultAsync(card =>
                        card.FlashcardId == id && card.UserId == userId);

                if (flashcard == null)
                {
                    return NotFound();
                }

                return View(flashcard);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to load flashcard {FlashcardId} for deletion.",
                    id);

                return StatusCode(
                    500,
                    "The flashcard could not be loaded. Please try again.");
            }
        }

        // Deletes a card after confirming its ownership.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var flashcard = await _db.Flashcards
                    .FirstOrDefaultAsync(card =>
                        card.FlashcardId == id && card.UserId == userId);

                if (flashcard == null)
                {
                    return NotFound();
                }

                _db.Flashcards.Remove(flashcard);
                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Deleted flashcard {FlashcardId}.", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Failed to delete flashcard {FlashcardId}.", id);

                return StatusCode(
                    500,
                    "The flashcard could not be deleted. Please try again.");
            }
        }

        // Displays one of the user's cards at a time for studying.
        [HttpGet]
        public async Task<IActionResult> Study(int index = 0)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var flashcards = await _db.Flashcards
                    .Where(card => card.UserId == userId)
                    .OrderBy(card => card.FlashcardId)
                    .ToListAsync();

                if (flashcards.Count == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (index < 0 || index >= flashcards.Count)
                {
                    index = 0;
                }

                ViewBag.Index = index;
                ViewBag.Total = flashcards.Count;

                return View(flashcards[index]);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Failed to load flashcard study mode.");

                return StatusCode(
                    500,
                    "Study mode could not be loaded. Please try again.");
            }
        }
    }
}