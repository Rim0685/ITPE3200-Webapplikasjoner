using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    // Implements database operations for a user's flashcards.
    public class FlashcardRepository : IFlashcardRepository
    {
        private readonly QuizDbContext _db;
        private readonly ILogger<FlashcardRepository> _logger;

        public FlashcardRepository(
            QuizDbContext db,
            ILogger<FlashcardRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Retrieves the user's flashcards, including their deck.
        public async Task<IEnumerable<Flashcard>?> GetAll(string userId)
        {
            try
            {
                return await _db.Flashcards
                    .AsNoTracking()
                    .Include(card => card.Deck)
                    .Where(card => card.UserId == userId)
                    .OrderByDescending(card => card.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardRepository] Failed to retrieve flashcards.");

                return null;
            }
        }

        // Retrieves a flashcard only if it belongs to the user.
        public async Task<Flashcard?> GetById(int id, string userId)
        {
            try
            {
                return await _db.Flashcards
                    .Include(card => card.Deck)
                    .FirstOrDefaultAsync(card =>
                        card.FlashcardId == id
                        && card.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardRepository] Failed to retrieve FlashcardId {FlashcardId}.",
                    id);

                return null;
            }
        }

        // Saves a flashcard and any new deck associated with it.
        public async Task<bool> Create(Flashcard flashcard)
        {
            try
            {
                _db.Flashcards.Add(flashcard);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardRepository] Failed to create a flashcard.");

                return false;
            }
        }

        // Updates editable fields while preserving ownership and creation time.
        public async Task<bool> Update(
            Flashcard flashcard, string userId)
        {
            try
            {
                var existingCard = await _db.Flashcards
                    .FirstOrDefaultAsync(card =>
                        card.FlashcardId == flashcard.FlashcardId
                        && card.UserId == userId);

                if (existingCard == null)
                {
                    _logger.LogWarning(
                        "[FlashcardRepository] FlashcardId {FlashcardId} was not available for updating.",
                        flashcard.FlashcardId);

                    return false;
                }

                existingCard.QuestionText = flashcard.QuestionText;
                existingCard.AnswerText = flashcard.AnswerText;
                existingCard.DeckId = flashcard.DeckId;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardRepository] Failed to update FlashcardId {FlashcardId}.",
                    flashcard.FlashcardId);

                return false;
            }
        }

        // Deletes a flashcard only if it belongs to the user.
        public async Task<bool> Delete(int id, string userId)
        {
            try
            {
                var flashcard = await _db.Flashcards
                    .FirstOrDefaultAsync(card =>
                        card.FlashcardId == id
                        && card.UserId == userId);

                if (flashcard == null)
                {
                    _logger.LogWarning(
                        "[FlashcardRepository] FlashcardId {FlashcardId} was not available for deletion.",
                        id);

                    return false;
                }

                _db.Flashcards.Remove(flashcard);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardRepository] Failed to delete FlashcardId {FlashcardId}.",
                    id);

                return false;
            }
        }
    }
}