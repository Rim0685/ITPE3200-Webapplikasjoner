// The DAL folder contains database configuration and data access code.
// This class handles database operations for a user's flashcards.

using Microsoft.EntityFrameworkCore;
using QuizApi.Models;

namespace QuizApi.DAL
{
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

        // Retrieves all flashcards belonging to the specified user.
        public async Task<IEnumerable<Flashcard>> GetAllAsync(string userId)
        {
            try
            {
                return await _db.Flashcards
                    .Where(f => f.UserId == userId)
                    .OrderBy(f => f.FlashcardId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve flashcards.");
                throw;
            }
        }

        // Retrieves a flashcard only if it belongs to the specified user.
        public async Task<Flashcard?> GetByIdAsync(
            int flashcardId, string userId)
        {
            try
            {
                return await _db.Flashcards
                    .FirstOrDefaultAsync(f =>
                        f.FlashcardId == flashcardId &&
                        f.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve flashcard {FlashcardId}.",
                    flashcardId);

                throw;
            }
        }

        // Creates a flashcard with the owner provided by the API.
        public async Task<bool> CreateAsync(Flashcard flashcard)
        {
            try
            {
                _db.Flashcards.Add(flashcard);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to create flashcard.");
                return false;
            }
        }

        // Updates the content without changing the owner.
        public async Task<bool> UpdateAsync(
            Flashcard flashcard, string userId)
        {
            try
            {
                var existingFlashcard = await _db.Flashcards
                    .FirstOrDefaultAsync(f =>
                        f.FlashcardId == flashcard.FlashcardId &&
                        f.UserId == userId);

                if (existingFlashcard == null)
                {
                    _logger.LogWarning(
                        "Flashcard {FlashcardId} was not found for update.",
                        flashcard.FlashcardId);

                    return false;
                }

                existingFlashcard.Front = flashcard.Front;
                existingFlashcard.Back = flashcard.Back;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update flashcard {FlashcardId}.",
                    flashcard.FlashcardId);

                return false;
            }
        }

        // Deletes a flashcard only if it belongs to the specified user.
        public async Task<bool> DeleteAsync(
            int flashcardId, string userId)
        {
            try
            {
                var flashcard = await _db.Flashcards
                    .FirstOrDefaultAsync(f =>
                        f.FlashcardId == flashcardId &&
                        f.UserId == userId);

                if (flashcard == null)
                {
                    _logger.LogWarning(
                        "Flashcard {FlashcardId} was not found for deletion.",
                        flashcardId);

                    return false;
                }

                _db.Flashcards.Remove(flashcard);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete flashcard {FlashcardId}.",
                    flashcardId);

                return false;
            }
        }
    }
}