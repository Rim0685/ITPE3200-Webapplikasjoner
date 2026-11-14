using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    // Implements database operations for a user's flashcard decks.
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

        // Retrieves the user's decks with their cards, newest first.
        public async Task<IEnumerable<FlashcardDeck>?> GetAll(
            string userId)
        {
            try
            {
                return await _db.FlashcardDecks
                    .AsNoTracking()
                    .Include(deck => deck.Flashcards)
                    .Where(deck => deck.UserId == userId)
                    .OrderByDescending(deck => deck.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardDeckRepository] Failed to retrieve flashcard decks.");

                return null;
            }
        }

        // Retrieves a deck only if it belongs to the specified user.
        public async Task<FlashcardDeck?> GetById(
            int id, string userId)
        {
            try
            {
                return await _db.FlashcardDecks
                    .Include(deck => deck.Flashcards)
                    .FirstOrDefaultAsync(deck =>
                        deck.FlashcardDeckId == id
                        && deck.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardDeckRepository] Failed to retrieve DeckId {DeckId}.",
                    id);

                return null;
            }
        }

        // Saves a new deck.
        public async Task<bool> Create(FlashcardDeck deck)
        {
            try
            {
                _db.FlashcardDecks.Add(deck);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardDeckRepository] Failed to create a flashcard deck.");

                return false;
            }
        }

        // Updates the name and description while preserving ownership.
        public async Task<bool> Update(
            FlashcardDeck deck, string userId)
        {
            try
            {
                var existing = await _db.FlashcardDecks
                    .FirstOrDefaultAsync(d =>
                        d.FlashcardDeckId == deck.FlashcardDeckId
                        && d.UserId == userId);

                if (existing == null)
                {
                    _logger.LogWarning(
                        "[FlashcardDeckRepository] DeckId {DeckId} was not available for updating.",
                        deck.FlashcardDeckId);

                    return false;
                }

                existing.Name = deck.Name;
                existing.Description = deck.Description;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardDeckRepository] Failed to update DeckId {DeckId}.",
                    deck.FlashcardDeckId);

                return false;
            }
        }

        // Deletes a deck and its cards only if it belongs to the user.
        public async Task<bool> Delete(int id, string userId)
        {
            try
            {
                var deck = await _db.FlashcardDecks
                    .Include(d => d.Flashcards)
                    .FirstOrDefaultAsync(d =>
                        d.FlashcardDeckId == id
                        && d.UserId == userId);

                if (deck == null)
                {
                    _logger.LogWarning(
                        "[FlashcardDeckRepository] DeckId {DeckId} was not available for deletion.",
                        id);

                    return false;
                }

                _db.FlashcardDecks.Remove(deck);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[FlashcardDeckRepository] Failed to delete DeckId {DeckId}.",
                    id);

                return false;
            }
        }
    }
}