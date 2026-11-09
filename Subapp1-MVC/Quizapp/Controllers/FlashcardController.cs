// The Controllers folder contains classes that handle web requests.
// This controller manages the authenticated user's flashcards.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public FlashcardController(
            QuizDbContext db,
            UserManager<ApplicationUser> userManager,
            ILogger<FlashcardController> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        // Fills the deck dropdown with the user's own decks.
        private async Task LoadDecksAsync(string userId, int? selectedDeckId)
        {
            var decks = await _db.FlashcardDecks
                .Where(deck => deck.UserId == userId)
                .OrderBy(deck => deck.Name)
                .ToListAsync();

            ViewBag.Decks = new SelectList(
                decks, "FlashcardDeckId", "Name", selectedDeckId);
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
                    .Include(card => card.Deck)
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
        public async Task<IActionResult> Create(int? deckId)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            await LoadDecksAsync(userId, deckId);

            return View(new Flashcard { DeckId = deckId });
        }

        // Creates a flashcard. If a new deck name is typed, the deck is
        // created first and the card is placed in it.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("QuestionText,AnswerText,DeckId")] Flashcard model,
            string? newDeckName)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            newDeckName = newDeckName?.Trim();

            if (!string.IsNullOrEmpty(newDeckName) && newDeckName.Length > 100)
            {
                ModelState.AddModelError(
                    "newDeckName",
                    "The deck name can be at most 100 characters.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDecksAsync(userId, model.DeckId);
                return View(model);
            }

            try
            {
                var flashcard = new Flashcard
                {
                    QuestionText = model.QuestionText,
                    AnswerText = model.AnswerText,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                if (!string.IsNullOrEmpty(newDeckName))
                {
                    // Creates the new deck and places the card in it.
                    flashcard.Deck = new FlashcardDeck
                    {
                        Name = newDeckName,
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                else if (model.DeckId != null)
                {
                    // Only allows decks owned by the current user.
                    var ownsDeck = await _db.FlashcardDecks.AnyAsync(deck =>
                        deck.FlashcardDeckId == model.DeckId
                        && deck.UserId == userId);

                    if (!ownsDeck)
                    {
                        return NotFound();
                    }

                    flashcard.DeckId = model.DeckId;
                }

                _db.Flashcards.Add(flashcard);
                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Created flashcard {FlashcardId}.",
                    flashcard.FlashcardId);

                if (flashcard.DeckId != null)
                {
                    return RedirectToAction(
                        "Details",
                        "FlashcardDeck",
                        new { id = flashcard.DeckId });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a flashcard.");

                ModelState.AddModelError(
                    string.Empty,
                    "The flashcard could not be saved. Please try again.");

                await LoadDecksAsync(userId, model.DeckId);
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

                await LoadDecksAsync(userId, flashcard.DeckId);

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

        // Updates the editable fields of a card owned by the user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            [Bind("FlashcardId,QuestionText,AnswerText,DeckId")] Flashcard model)
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
                    await LoadDecksAsync(userId, model.DeckId);
                    return View(model);
                }

                if (model.DeckId != null)
                {
                    var ownsDeck = await _db.FlashcardDecks.AnyAsync(deck =>
                        deck.FlashcardDeckId == model.DeckId
                        && deck.UserId == userId);

                    if (!ownsDeck)
                    {
                        return NotFound();
                    }
                }

                flashcard.QuestionText = model.QuestionText;
                flashcard.AnswerText = model.AnswerText;
                flashcard.DeckId = model.DeckId;

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

                var userId = _userManager.GetUserId(User);
                if (!string.IsNullOrEmpty(userId))
                {
                    await LoadDecksAsync(userId, model.DeckId);
                }

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

        // Displays one card at a time. If deckId is given, only that
        // deck's cards are studied.
        [HttpGet]
        public async Task<IActionResult> Study(int? deckId, int index = 0)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var query = _db.Flashcards
                    .Where(card => card.UserId == userId);

                if (deckId != null)
                {
                    query = query.Where(card => card.DeckId == deckId);
                }

                var flashcards = await query
                    .OrderBy(card => card.FlashcardId)
                    .ToListAsync();

                if (flashcards.Count == 0)
                {
                    return deckId != null
                        ? RedirectToAction(
                            "Details", "FlashcardDeck", new { id = deckId })
                        : RedirectToAction(nameof(Index));
                }

                if (index < 0 || index >= flashcards.Count)
                {
                    index = 0;
                }

                ViewBag.Index = index;
                ViewBag.Total = flashcards.Count;
                ViewBag.DeckId = deckId;

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