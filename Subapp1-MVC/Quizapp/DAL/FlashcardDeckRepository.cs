// The DAL folder contains database configuration and data access code.
// This class implements database operations for a user's flashcard decks.

using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    public class FlashcardDeckRepository : IFlashcardDeckRepository
    {
        private readonly QuizDbContext _db;

        // Receives the database context through dependency injection.
        public FlashcardDeckRepository(QuizDbContext db)
        {
            _db = db;
        }

        // Retrieves the user's decks with their cards, newest first.
        public async Task<IEnumerable<FlashcardDeck>> GetAll(string userId)
        {
            return await _db.FlashcardDecks
                .AsNoTracking()
                .Include(deck => deck.Flashcards)
                .Where(deck => deck.UserId == userId)
                .OrderByDescending(deck => deck.CreatedAt)
                .ToListAsync();
        }

        // Retrieves a deck only if it belongs to the specified user.
        public async Task<FlashcardDeck?> GetById(int id, string userId)
        {
            return await _db.FlashcardDecks
                .Include(deck => deck.Flashcards)
                .FirstOrDefaultAsync(deck =>
                    deck.FlashcardDeckId == id
                    && deck.UserId == userId);
        }

        // Saves a new deck.
        public async Task Create(FlashcardDeck deck)
        {
            _db.FlashcardDecks.Add(deck);
            await _db.SaveChangesAsync();
        }

        // Updates only the name and description of a deck owned by the user.
        public async Task<bool> Update(FlashcardDeck deck, string userId)
        {
            var existing = await _db.FlashcardDecks
                .FirstOrDefaultAsync(d =>
                    d.FlashcardDeckId == deck.FlashcardDeckId
                    && d.UserId == userId);

            if (existing == null)
            {
                return false;
            }

            existing.Name = deck.Name;
            existing.Description = deck.Description;

            await _db.SaveChangesAsync();

            return true;
        }

        // Deletes a deck and its cards only if it belongs to the user.
        public async Task<bool> Delete(int id, string userId)
        {
            var deck = await _db.FlashcardDecks
                .Include(d => d.Flashcards)
                .FirstOrDefaultAsync(d =>
                    d.FlashcardDeckId == id && d.UserId == userId);

            if (deck == null)
            {
                return false;
            }

            _db.FlashcardDecks.Remove(deck);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}