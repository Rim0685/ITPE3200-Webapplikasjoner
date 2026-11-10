// The Services folder contains the application's business logic.
// This interface defines the operations available for a user's decks.

using QuizApi.DTOs;

namespace QuizApi.Services
{
    public interface IFlashcardDeckService
    {
        Task<IEnumerable<FlashcardDeckDto>> GetAllAsync(string userId);

        Task<FlashcardDeckDto?> GetByIdAsync(int deckId, string userId);

        Task<FlashcardDeckResult> CreateAsync(CreateFlashcardDeckDto dto, string userId);

        Task<FlashcardDeckResult> UpdateAsync(int deckId, UpdateFlashcardDeckDto dto, string userId);

        Task<FlashcardDeckResult> DeleteAsync(int deckId, string userId);
    }
}