using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers
{
    [Authorize]
    public class FlashcardController : Controller
    {
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IFlashcardDeckRepository _deckRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FlashcardController> _logger;

        public FlashcardController(
            IFlashcardRepository flashcardRepository,
            IFlashcardDeckRepository deckRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<FlashcardController> logger)
        {
            _flashcardRepository = flashcardRepository;
            _deckRepository = deckRepository;
            _userManager = userManager;
            _logger = logger;
        }

        // Displays the user's flashcards.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var flashcards = await _flashcardRepository.GetAll(userId);

            if (flashcards == null)
            {
                _logger.LogError(
                    "[FlashcardController] Failed to retrieve flashcards.");

                return StatusCode(
                    500,
                    "The flashcards could not be loaded. Please try again.");
            }

            var model = flashcards
                .Select(card => ToViewModel(card))
                .ToList();

            return View(model);
        }

        // Displays the creation form with the user's available decks.
        [HttpGet]
        public async Task<IActionResult> Create(int? deckId)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (deckId.HasValue)
            {
                var deck = await _deckRepository.GetById(
                    deckId.Value, userId);

                if (deck == null)
                {
                    return NotFound("The deck was not found.");
                }
            }

            var model = new FlashcardViewModel
            {
                DeckId = deckId
            };

            bool decksLoaded = await LoadDecksAsync(userId, model);

            if (!decksLoaded)
            {
                return StatusCode(
                    500,
                    "The decks could not be loaded. Please try again.");
            }

            return View(model);
        }

        // Creates a flashcard, optionally together with a new deck.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlashcardViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (ModelState.IsValid)
            {
                var flashcard = new Flashcard
                {
                    QuestionText = model.QuestionText,
                    AnswerText = model.AnswerText,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                if (!string.IsNullOrWhiteSpace(model.NewDeckName))
                {
                    // Saves the new deck and flashcard together.
                    flashcard.Deck = new FlashcardDeck
                    {
                        Name = model.NewDeckName.Trim(),
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                else if (model.DeckId.HasValue)
                {
                    var deck = await _deckRepository.GetById(
                        model.DeckId.Value, userId);

                    if (deck == null)
                    {
                        return NotFound("The deck was not found.");
                    }

                    flashcard.DeckId = deck.FlashcardDeckId;
                }

                bool returnOk = await _flashcardRepository.Create(
                    flashcard);

                if (returnOk)
                {
                    _logger.LogInformation(
                        "[FlashcardController] Created FlashcardId {FlashcardId}.",
                        flashcard.FlashcardId);

                    if (flashcard.DeckId.HasValue)
                    {
                        return RedirectToAction(
                            "Details",
                            "FlashcardDeck",
                            new { id = flashcard.DeckId });
                    }

                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning(
                    "[FlashcardController] Flashcard creation failed.");

                ModelState.AddModelError(
                    string.Empty,
                    "The flashcard could not be saved. Please try again.");
            }

            bool decksLoaded = await LoadDecksAsync(userId, model);

            if (!decksLoaded)
            {
                return StatusCode(
                    500,
                    "The decks could not be loaded. Please try again.");
            }

            return View(model);
        }

        // Displays the edit form for a flashcard owned by the user.
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var flashcard = await _flashcardRepository.GetById(
                id, userId);

            if (flashcard == null)
            {
                _logger.LogWarning(
                    "[FlashcardController] FlashcardId {FlashcardId} was not available for editing.",
                    id);

                return NotFound("The flashcard was not found.");
            }

            var model = ToViewModel(flashcard);

            bool decksLoaded = await LoadDecksAsync(userId, model);

            if (!decksLoaded)
            {
                return StatusCode(
                    500,
                    "The decks could not be loaded. Please try again.");
            }

            return View(model);
        }

        // Updates a flashcard after checking card and deck ownership.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(FlashcardViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var flashcard = await _flashcardRepository.GetById(
                model.FlashcardId, userId);

            if (flashcard == null)
            {
                _logger.LogWarning(
                    "[FlashcardController] FlashcardId {FlashcardId} was not available for editing.",
                    model.FlashcardId);

                return NotFound("The flashcard was not found.");
            }

            // Restores the stored creation time for redisplaying the form.
            model.CreatedAt = flashcard.CreatedAt;

            if (ModelState.IsValid)
            {
                if (model.DeckId.HasValue)
                {
                    var deck = await _deckRepository.GetById(
                        model.DeckId.Value, userId);

                    if (deck == null)
                    {
                        return NotFound("The deck was not found.");
                    }
                }

                bool returnOk = await _flashcardRepository.Update(
                    new Flashcard
                    {
                        FlashcardId = model.FlashcardId,
                        QuestionText = model.QuestionText,
                        AnswerText = model.AnswerText,
                        DeckId = model.DeckId
                    },
                    userId);

                if (returnOk)
                {
                    _logger.LogInformation(
                        "[FlashcardController] Updated FlashcardId {FlashcardId}.",
                        model.FlashcardId);

                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning(
                    "[FlashcardController] Update failed for FlashcardId {FlashcardId}.",
                    model.FlashcardId);

                ModelState.AddModelError(
                    string.Empty,
                    "The changes could not be saved. Please try again.");
            }

            bool decksLoaded = await LoadDecksAsync(userId, model);

            if (!decksLoaded)
            {
                return StatusCode(
                    500,
                    "The decks could not be loaded. Please try again.");
            }

            return View(model);
        }

        // Displays a confirmation page before deleting a flashcard.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var flashcard = await _flashcardRepository.GetById(
                id, userId);

            if (flashcard == null)
            {
                _logger.LogWarning(
                    "[FlashcardController] FlashcardId {FlashcardId} was not available for deletion.",
                    id);

                return NotFound("The flashcard was not found.");
            }

            return View(ToViewModel(flashcard));
        }

        // Deletes a flashcard owned by the user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            bool returnOk = await _flashcardRepository.Delete(id, userId);

            if (!returnOk)
            {
                _logger.LogWarning(
                    "[FlashcardController] Deletion failed for FlashcardId {FlashcardId}.",
                    id);

                return BadRequest("The flashcard could not be deleted.");
            }

            _logger.LogInformation(
                "[FlashcardController] Deleted FlashcardId {FlashcardId}.",
                id);

            return RedirectToAction(nameof(Index));
        }

        // Displays one flashcard at a time.
        [HttpGet]
        public async Task<IActionResult> Study(
            int? deckId, int index = 0)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (deckId.HasValue)
            {
                var deck = await _deckRepository.GetById(
                    deckId.Value, userId);

                if (deck == null)
                {
                    return NotFound("The deck was not found.");
                }
            }

            var cards = await _flashcardRepository.GetAll(userId);

            if (cards == null)
            {
                _logger.LogError(
                    "[FlashcardController] Failed to retrieve cards for study mode.");

                return StatusCode(
                    500,
                    "Study mode could not be loaded. Please try again.");
            }

            if (deckId.HasValue)
            {
                cards = cards.Where(card => card.DeckId == deckId.Value);
            }

            var flashcards = cards
                .OrderBy(card => card.FlashcardId)
                .ToList();

            if (flashcards.Count == 0)
            {
                if (deckId.HasValue)
                {
                    return RedirectToAction(
                        "Details",
                        "FlashcardDeck",
                        new { id = deckId });
                }

                return RedirectToAction(nameof(Index));
            }

            if (index < 0 || index >= flashcards.Count)
            {
                index = 0;
            }

            var model = ToViewModel(flashcards[index]);

            model.StudyIndex = index;
            model.StudyTotal = flashcards.Count;

            // Preserves the current study filter for navigation.
            model.DeckId = deckId;

            return View(model);
        }

        // Loads the user's decks into the form view model.
        private async Task<bool> LoadDecksAsync(
            string userId, FlashcardViewModel model)
        {
            var decks = await _deckRepository.GetAll(userId);

            if (decks == null)
            {
                _logger.LogError(
                    "[FlashcardController] Failed to retrieve decks for the form.");

                return false;
            }

            model.Decks = decks
                .OrderBy(deck => deck.Name)
                .Select(deck => new SelectListItem
                {
                    Value = deck.FlashcardDeckId.ToString(),
                    Text = deck.Name,
                    Selected = deck.FlashcardDeckId == model.DeckId
                }).ToList();

            return true;
        }

        // Converts a flashcard to data used by the views.
        private static FlashcardViewModel ToViewModel(Flashcard card)
        {
            return new FlashcardViewModel
            {
                FlashcardId = card.FlashcardId,
                QuestionText = card.QuestionText,
                AnswerText = card.AnswerText,
                CreatedAt = card.CreatedAt,
                DeckId = card.DeckId,
                DeckName = card.Deck?.Name
            };
        }
    }
}