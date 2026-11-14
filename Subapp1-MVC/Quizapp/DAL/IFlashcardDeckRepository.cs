// The DAL folder contains database configuration and data access code.
// This interface defines database operations for a user's flashcard decks.

using Quizapp.Models;

namespace Quizapp.DAL
{
    // Defines database operations for a user's flashcard decks.
    public interface IFlashcardDeckRepository
    {
        // Retrieves all decks belonging to the specified user.
        Task<IEnumerable<FlashcardDeck>?> GetAll(string userId);

        // Retrieves a deck with its cards, only if it belongs to the user.
        Task<FlashcardDeck?> GetById(int id, string userId);

        // Saves a new deck.
        Task<bool> Create(FlashcardDeck deck);

        // Updates an existing deck belonging to the specified user.
        Task<bool> Update(FlashcardDeck deck, string userId);

        // Deletes a deck and its cards only if it belongs to the user.
        Task<bool> Delete(int id, string userId);
    }
}