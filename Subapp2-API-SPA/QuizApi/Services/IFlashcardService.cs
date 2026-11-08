// The Services folder contains the application's business logic.
// This interface defines the operations available for a user's flashcards.

using QuizApi.DTOs;

namespace QuizApi.Services
{
    public interface IFlashcardService
    {
        Task<IEnumerable<FlashcardDto>> GetAllAsync(string userId);

        Task<FlashcardDto?> GetByIdAsync(int flashcardId, string userId);

        Task<FlashcardResult> CreateAsync(CreateFlashcardDto dto, string userId);

        Task<FlashcardResult> UpdateAsync(int flashcardId, UpdateFlashcardDto dto, string userId);

        Task<FlashcardResult> DeleteAsync(int flashcardId, string userId);
    }
}
