// The DAL folder contains database configuration and data access code.
// This interface defines database operations for a user's flashcards.

using QuizApi.Models;

namespace QuizApi.DAL
{
    public interface IFlashcardRepository
    {
        Task<IEnumerable<Flashcard>> GetAllAsync(string userId);

        Task<Flashcard?> GetByIdAsync(int flashcardId, string userId);

        Task<bool> CreateAsync(Flashcard flashcard);

        Task<bool> UpdateAsync(Flashcard flashcard, string userId);

        Task<bool> DeleteAsync(int flashcardId, string userId);
    }
}