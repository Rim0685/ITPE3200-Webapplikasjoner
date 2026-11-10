// The DAL folder contains database configuration and data access code.
// This class handles database operations for a user's flashcard decks.

using Microsoft.EntityFrameworkCore;
using QuizApi.Models;

namespace QuizApi.DAL
{
    public class FlashcardDeckRepository : IFlashcardDeckRepository
    {
        private readonly QuizDbContext _db;
        private readonly ILogger<FlashcardDeckRepository> _logger;

        public FlashcardDeckRepository(
            QuizDbContext db,
            ILogger<FlashcardDeckRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Retrieves all decks belonging to the user, with their flashcards.
        public async Task<IEnumerable<FlashcardDeck>> GetAllAsync(string userId)
        {
            try
            {
                return await _db.FlashcardDecks
                    .Include(d => d.Flashcards)
                    .Where(d => d.UserId == userId)
                    .OrderBy(d => d.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve flashcard decks.");
                throw;
            }
        }

        // Retrieves a deck only if it belongs to the specified user.
        public async Task<FlashcardDeck?> GetByIdAsync(int deckId, string userId)
        {
            try
            {
                return await _db.FlashcardDecks
                    .Include(d => d.Flashcards)
                    .FirstOrDefaultAsync(d =>
                        d.FlashcardDeckId == deckId &&
                        d.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve deck {DeckId}.", deckId);
                throw;
            }
        }

        // Counts the decks belonging to the specified user.
        public async Task<int> CountAsync(string userId)
        {
            try
            {
                return await _db.FlashcardDecks.CountAsync(d => d.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to count flashcard decks.");
                throw;
            }
        }

        // Compares names without regard to upper or lower case.
        public async Task<bool> NameExistsAsync(
            string userId, string name, int? excludeDeckId = null)
        {
            try
            {
                var lowerName = name.ToLower();

                return await _db.FlashcardDecks.AnyAsync(d =>
                    d.UserId == userId &&
                    d.Name.ToLower() == lowerName &&
                    (excludeDeckId == null || d.FlashcardDeckId != excludeDeckId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check the deck name.");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int deckId, string userId)
        {
            try
            {
                return await _db.FlashcardDecks.AnyAsync(d =>
                    d.FlashcardDeckId == deckId &&
                    d.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check deck {DeckId}.", deckId);
                throw;
            }
        }

        // Creates a deck with the owner provided by the API.
        public async Task<bool> CreateAsync(FlashcardDeck deck)
        {
            try
            {
                _db.FlashcardDecks.Add(deck);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to create flashcard deck.");
                return false;
            }
        }

        // Updates name and description without changing the owner.
        public async Task<bool> UpdateAsync(FlashcardDeck deck, string userId)
        {
            try
            {
                var existing = await _db.FlashcardDecks
                    .FirstOrDefaultAsync(d =>
                        d.FlashcardDeckId == deck.FlashcardDeckId &&
                        d.UserId == userId);

                if (existing == null)
                {
                    _logger.LogWarning(
                        "Deck {DeckId} was not found for update.",
                        deck.FlashcardDeckId);

                    return false;
                }

                existing.Name = deck.Name;
                existing.Description = deck.Description;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex, "Failed to update deck {DeckId}.", deck.FlashcardDeckId);

                return false;
            }
        }

        // Deletes a deck and its flashcards, only if the user owns it.
        public async Task<bool> DeleteAsync(int deckId, string userId)
        {
            try
            {
                var deck = await _db.FlashcardDecks
                    .FirstOrDefaultAsync(d =>
                        d.FlashcardDeckId == deckId &&
                        d.UserId == userId);

                if (deck == null)
                {
                    _logger.LogWarning(
                        "Deck {DeckId} was not found for deletion.", deckId);

                    return false;
                }

                _db.FlashcardDecks.Remove(deck);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete deck {DeckId}.", deckId);
                return false;
            }
        }
    }
}