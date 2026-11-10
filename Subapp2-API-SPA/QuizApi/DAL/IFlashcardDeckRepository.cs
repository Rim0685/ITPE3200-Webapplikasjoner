// The DAL folder contains database configuration and data access code.
// This interface defines database operations for a user's flashcard decks.

using QuizApi.Models;

namespace QuizApi.DAL
{
    public interface IFlashcardDeckRepository
    {
        Task<IEnumerable<FlashcardDeck>> GetAllAsync(string userId);

        Task<FlashcardDeck?> GetByIdAsync(int deckId, string userId);

        Task<int> CountAsync(string userId);

        // Checks if the user already has a deck with this name (ignores case).
        Task<bool> NameExistsAsync(string userId, string name, int? excludeDeckId = null);

        // Checks if the deck exists and belongs to the user.
        Task<bool> ExistsAsync(int deckId, string userId);

        Task<bool> CreateAsync(FlashcardDeck deck);

        Task<bool> UpdateAsync(FlashcardDeck deck, string userId);

        Task<bool> DeleteAsync(int deckId, string userId);
    }
}