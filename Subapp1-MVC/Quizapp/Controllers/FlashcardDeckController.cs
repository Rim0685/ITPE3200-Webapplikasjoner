// The Controllers folder contains classes that handle web requests.
// This controller manages flashcard decks belonging to the signed-in user.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers
{
    [Authorize]
    public class FlashcardDeckController : Controller
    {
        private readonly IFlashcardDeckRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FlashcardDeckController> _logger;

        // Receives repository, user management and logging services.
        public FlashcardDeckController(
            IFlashcardDeckRepository repository,
            UserManager<ApplicationUser> userManager,
            ILogger<FlashcardDeckController> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _logger = logger;
        }

        // Displays the user's decks.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var decks = await _repository.GetAll(userId);

                var model = decks.Select(deck => new FlashcardDeckViewModel
                {
                    FlashcardDeckId = deck.FlashcardDeckId,
                    Name = deck.Name,
                    Description = deck.Description,
                    CreatedAt = deck.CreatedAt,
                    CardCount = deck.Flashcards.Count
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load flashcard decks.");

                return StatusCode(
                    500, "The decks could not be loaded. Please try again.");
            }
        }

        // Displays a deck and its cards, only if the user owns it.
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var deck = await _repository.GetById(id, userId);

                if (deck == null)
                {
                    return NotFound();
                }

                return View(ToViewModel(deck, includeCards: true));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Failed to load flashcard deck {DeckId}.", id);

                return StatusCode(
                    500, "The deck could not be loaded. Please try again.");
            }
        }

        // Displays an empty creation form.
        [HttpGet]
        public IActionResult Create()
        {
            return View(new FlashcardDeckViewModel());
        }

        // Creates a deck with ownership and creation time set on the server.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlashcardDeckViewModel model)
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

                var deck = new FlashcardDeck
                {
                    Name = model.Name,
                    Description = model.Description,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.Create(deck);

                _logger.LogInformation(
                    "Created flashcard deck {DeckId}.",
                    deck.FlashcardDeckId);

                return RedirectToAction(
                    nameof(Details), new { id = deck.FlashcardDeckId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a flashcard deck.");

                ModelState.AddModelError(
                    string.Empty,
                    "The deck could not be saved. Please try again.");

                return View(model);
            }
        }

        // Displays the edit form for a deck owned by the user.
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

                var deck = await _repository.GetById(id, userId);

                if (deck == null)
                {
                    return NotFound();
                }

                return View(ToViewModel(deck, includeCards: false));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to load flashcard deck {DeckId} for editing.",
                    id);

                return StatusCode(
                    500, "The deck could not be loaded. Please try again.");
            }
        }

        // Updates name and description after checking ownership.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(FlashcardDeckViewModel model)
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

                // The repository only changes name and description.
                bool updated = await _repository.Update(
                    new FlashcardDeck
                    {
                        FlashcardDeckId = model.FlashcardDeckId,
                        Name = model.Name,
                        Description = model.Description
                    },
                    userId);

                if (!updated)
                {
                    return NotFound();
                }

                _logger.LogInformation(
                    "Updated flashcard deck {DeckId}.",
                    model.FlashcardDeckId);

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.FlashcardDeckId });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update flashcard deck {DeckId}.",
                    model.FlashcardDeckId);

                ModelState.AddModelError(
                    string.Empty,
                    "The changes could not be saved. Please try again.");

                return View(model);
            }
        }

        // Displays a confirmation page before deleting a deck.
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

                var deck = await _repository.GetById(id, userId);

                if (deck == null)
                {
                    return NotFound();
                }

                return View(ToViewModel(deck, includeCards: false));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to load flashcard deck {DeckId} for deletion.",
                    id);

                return StatusCode(
                    500, "The deck could not be loaded. Please try again.");
            }
        }

        // Deletes a deck and its cards after verifying ownership.
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

                bool deleted = await _repository.Delete(id, userId);

                if (!deleted)
                {
                    return NotFound();
                }

                _logger.LogInformation(
                    "Deleted flashcard deck {DeckId}.", id);

                TempData["SuccessMessage"] = "The deck has been deleted.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Failed to delete flashcard deck {DeckId}.", id);

                return StatusCode(
                    500, "The deck could not be deleted. Please try again.");
            }
        }

        // Converts a deck to a view model, optionally with its cards.
        private static FlashcardDeckViewModel ToViewModel(
            FlashcardDeck deck, bool includeCards)
        {
            return new FlashcardDeckViewModel
            {
                FlashcardDeckId = deck.FlashcardDeckId,
                Name = deck.Name,
                Description = deck.Description,
                CreatedAt = deck.CreatedAt,
                CardCount = deck.Flashcards.Count,
                Cards = includeCards
                    ? deck.Flashcards
                        .OrderByDescending(card => card.CreatedAt)
                        .Select(card => new FlashcardSummaryViewModel
                        {
                            FlashcardId = card.FlashcardId,
                            QuestionText = card.QuestionText,
                            CreatedAt = card.CreatedAt
                        }).ToList()
                    : new List<FlashcardSummaryViewModel>()
            };
        }
    }
}