using Quizapp.Models;

namespace Quizapp.DAL
{
    // Defines database operations for a user's flashcards.
    public interface IFlashcardRepository
    {
        Task<IEnumerable<Flashcard>?> GetAll(string userId);

        Task<Flashcard?> GetById(int id, string userId);

        Task<bool> Create(Flashcard flashcard);

        Task<bool> Update(Flashcard flashcard, string userId);

        Task<bool> Delete(int id, string userId);
    }
}