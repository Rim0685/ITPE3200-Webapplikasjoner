using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;

namespace Quizapp.Controllers
{
    // Handles everything related to flashcards:
    // listing, creating, editing, deleting and studying cards.
    public class FlashcardController : Controller
    {
        private readonly QuizDbContext _db;
        private readonly ILogger<FlashcardController> _logger;

        // The database context and logger are provided through dependency injection (see Program.cs).
        public FlashcardController(QuizDbContext db, ILogger<FlashcardController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Shows a list of all flashcards.
        public IActionResult Index()
        {
            try
            {
                List<Flashcard> flashcards = _db.Flashcards.ToList();
                return View(flashcards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardController] Failed to load the flashcard list.");
                return View("Error");
            }
        }

        // Shows an empty form for creating a new flashcard.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Receives the form and saves the new card in the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Flashcard flashcard)
        {
            // Shows the form again with error messages if a field is missing or too long.
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[FlashcardController] Invalid input when creating a flashcard.");
                return View(flashcard);
            }

            try
            {
                _db.Flashcards.Add(flashcard);
                _db.SaveChanges();
                _logger.LogInformation("[FlashcardController] Created flashcard {FlashcardId}.", flashcard.FlashcardId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardController] Failed to create a flashcard.");
                return View("Error");
            }
        }

        // Loads the card to edit and shows the form with its current values.
        [HttpGet]
        public IActionResult Update(int id)
        {
            try
            {
                var flashcard = _db.Flashcards.Find(id);

                // Returns 404 if no card has this ID.
                if (flashcard == null)
                {
                    _logger.LogWarning("[FlashcardController] Flashcard {FlashcardId} not found for update.", id);
                    return NotFound();
                }
                return View(flashcard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardController] Failed to load flashcard {FlashcardId} for update.", id);
                return View("Error");
            }
        }

        // Saves the changes to an existing card.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Flashcard flashcard)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[FlashcardController] Invalid input when updating flashcard {FlashcardId}.", flashcard.FlashcardId);
                return View(flashcard);
            }

            try
            {
                _db.Flashcards.Update(flashcard);
                _db.SaveChanges();
                _logger.LogInformation("[FlashcardController] Updated flashcard {FlashcardId}.", flashcard.FlashcardId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardController] Failed to update flashcard {FlashcardId}.", flashcard.FlashcardId);
                return View("Error");
            }
        }

        // Shows a confirmation page before the card is deleted.
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var flashcard = _db.Flashcards.Find(id);
                if (flashcard == null)
                {
                    _logger.LogWarning("[FlashcardController] Flashcard {FlashcardId} not found for delete.", id);
                    return NotFound();
                }
                return View(flashcard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardController] Failed to load flashcard {FlashcardId} for delete.", id);
                return View("Error");
            }
        }

        // Deletes the card after the user has confirmed.
        // Named DeleteConfirmed because Delete(int id) already exists with the same parameter.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var flashcard = _db.Flashcards.Find(id);
                if (flashcard == null)
                {
                    _logger.LogWarning("[FlashcardController] Flashcard {FlashcardId} not found when confirming delete.", id);
                    return NotFound();
                }

                _db.Flashcards.Remove(flashcard);
                _db.SaveChanges();
                _logger.LogInformation("[FlashcardController] Deleted flashcard {FlashcardId}.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardController] Failed to delete flashcard {FlashcardId}.", id);
                return View("Error");
            }
        }

        // Study mode: shows one card at a time. "index" decides which card is shown (0 = first).
        [HttpGet]
        public IActionResult Study(int index = 0)
        {
            try
            {
                // Sorted by ID so the order is the same every time.
                List<Flashcard> flashcards = _db.Flashcards
                    .OrderBy(f => f.FlashcardId)
                    .ToList();

                // No cards to study, so the user is sent back to the list.
                if (flashcards.Count == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                // Starts from the first card again after the last one (or if the index is invalid).
                if (index < 0 || index >= flashcards.Count)
                {
                    index = 0;
                }

                // Sends the position and number of cards to the view, so it can show "Card 2 of 5".
                ViewBag.Index = index;
                ViewBag.Total = flashcards.Count;
                return View(flashcards[index]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardController] Failed to load study mode (index {Index}).", index);
                return View("Error");
            }
        }
    }
}
