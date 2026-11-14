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
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var decks = await _repository.GetAll(userId);

            if (decks == null)
            {
                _logger.LogError(
                    "[FlashcardDeckController] Failed to retrieve flashcard decks.");

                return StatusCode(
                    500,
                    "The decks could not be loaded. Please try again.");
            }

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

        // Displays a deck and its cards only if the user owns it.
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var deck = await _repository.GetById(id, userId);

            if (deck == null)
            {
                _logger.LogWarning(
                    "[FlashcardDeckController] DeckId {DeckId} was not available.",
                    id);

                return NotFound("The deck was not found.");
            }

            return View(ToViewModel(deck, includeCards: true));
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
        public async Task<IActionResult> Create(
            FlashcardDeckViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (ModelState.IsValid)
            {
                var deck = new FlashcardDeck
                {
                    Name = model.Name,
                    Description = model.Description,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                bool returnOk = await _repository.Create(deck);

                if (returnOk)
                {
                    _logger.LogInformation(
                        "[FlashcardDeckController] Created DeckId {DeckId}.",
                        deck.FlashcardDeckId);

                    return RedirectToAction(
                        nameof(Details),
                        new { id = deck.FlashcardDeckId });
                }

                _logger.LogWarning(
                    "[FlashcardDeckController] Flashcard deck creation failed.");

                ModelState.AddModelError(
                    string.Empty,
                    "The deck could not be saved. Please try again.");
            }

            return View(model);
        }

        // Displays the edit form for a deck owned by the user.
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var deck = await _repository.GetById(id, userId);

            if (deck == null)
            {
                _logger.LogWarning(
                    "[FlashcardDeckController] DeckId {DeckId} was not available for editing.",
                    id);

                return NotFound("The deck was not found.");
            }

            return View(ToViewModel(deck, includeCards: false));
        }

        // Updates the editable fields of a deck owned by the user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            FlashcardDeckViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var deck = await _repository.GetById(
                model.FlashcardDeckId, userId);

            if (deck == null)
            {
                _logger.LogWarning(
                    "[FlashcardDeckController] DeckId {DeckId} was not available for editing.",
                    model.FlashcardDeckId);

                return NotFound("The deck was not found.");
            }

            // Restores display information from the database.
            model.CreatedAt = deck.CreatedAt;
            model.CardCount = deck.Flashcards.Count;

            if (ModelState.IsValid)
            {
                bool returnOk = await _repository.Update(
                    new FlashcardDeck
                    {
                        FlashcardDeckId = model.FlashcardDeckId,
                        Name = model.Name,
                        Description = model.Description
                    },
                    userId);

                if (returnOk)
                {
                    _logger.LogInformation(
                        "[FlashcardDeckController] Updated DeckId {DeckId}.",
                        model.FlashcardDeckId);

                    return RedirectToAction(
                        nameof(Details),
                        new { id = model.FlashcardDeckId });
                }

                _logger.LogWarning(
                    "[FlashcardDeckController] Update failed for DeckId {DeckId}.",
                    model.FlashcardDeckId);

                ModelState.AddModelError(
                    string.Empty,
                    "The changes could not be saved. Please try again.");
            }

            return View(model);
        }

        // Displays a confirmation page before deleting a deck.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var deck = await _repository.GetById(id, userId);

            if (deck == null)
            {
                _logger.LogWarning(
                    "[FlashcardDeckController] DeckId {DeckId} was not available for deletion.",
                    id);

                return NotFound("The deck was not found.");
            }

            return View(ToViewModel(deck, includeCards: false));
        }

        // Deletes a deck and its cards after checking ownership in the repository.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            bool returnOk = await _repository.Delete(id, userId);

            if (!returnOk)
            {
                _logger.LogWarning(
                    "[FlashcardDeckController] Deletion failed for DeckId {DeckId}.",
                    id);

                return BadRequest("The deck could not be deleted.");
            }

            _logger.LogInformation(
                "[FlashcardDeckController] Deleted DeckId {DeckId}.",
                id);

            TempData["SuccessMessage"] = "The deck has been deleted.";

            return RedirectToAction(nameof(Index));
        }

        // Converts a deck to a view model, optionally including its cards.
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