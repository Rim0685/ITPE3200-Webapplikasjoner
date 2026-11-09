// Defines database operations for a user's flashcards.

using Quizapp.Models;

namespace Quizapp.DAL
{
    public interface IFlashcardRepository
    {
        // Retrieves all flashcards belonging to the user.
        Task<IEnumerable<Flashcard>> GetAll(string userId);

        // Retrieves a flashcard only if it belongs to the user.
        Task<Flashcard?> GetById(int id, string userId);

        // Saves a new flashcard.
        Task Create(Flashcard flashcard);

        // Updates the editable fields of a flashcard owned by the user.
        Task<bool> Update(Flashcard flashcard, string userId);

        // Deletes a flashcard owned by the user.
        Task<bool> Delete(int id, string userId);
    }
}