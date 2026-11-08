// The Services folder contains the application's business logic.
// This class enforces the flashcard rules and maps entities to DTOs.
// All database access goes through the repository.

using QuizApi.DAL;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Services
{
    public class FlashcardService : IFlashcardService
    {
        // The maximum number of flashcards a single user can own.
        public const int MaxFlashcardsPerUser = 20;

        private readonly IFlashcardRepository _flashcardRepository;
        private readonly ILogger<FlashcardService> _logger;

        public FlashcardService(
            IFlashcardRepository flashcardRepository,
            ILogger<FlashcardService> logger)
        {
            _flashcardRepository = flashcardRepository;
            _logger = logger;
        }

        // Retrieves all flashcards belonging to the user.
        public async Task<IEnumerable<FlashcardDto>> GetAllAsync(string userId)
        {
            var flashcards = await _flashcardRepository.GetAllAsync(userId);

            return flashcards.Select(MapToDto);
        }

        // Retrieves a flashcard, or null if the user does not own one with this ID.
        public async Task<FlashcardDto?> GetByIdAsync(int flashcardId, string userId)
        {
            var flashcard = await _flashcardRepository.GetByIdAsync(flashcardId, userId);

            return flashcard == null ? null : MapToDto(flashcard);
        }

        // Creates a flashcard unless the user has reached the limit.
        public async Task<FlashcardResult> CreateAsync(CreateFlashcardDto dto, string userId)
        {
            var count = await _flashcardRepository.CountAsync(userId);

            if (count >= MaxFlashcardsPerUser)
            {
                _logger.LogWarning(
                    "[FlashcardService] User {UserId} has reached the flashcard limit.",
                    userId);

                return new FlashcardResult { Status = FlashcardResultStatus.LimitReached };
            }

            // The owner comes from the caller's token, never from the client's data.
            var flashcard = new Flashcard
            {
                Front = dto.Front.Trim(),
                Back = dto.Back.Trim(),
                UserId = userId
            };

            var created = await _flashcardRepository.CreateAsync(flashcard);

            if (!created)
            {
                return new FlashcardResult { Status = FlashcardResultStatus.Failed };
            }

            return new FlashcardResult
            {
                Status = FlashcardResultStatus.Success,
                Flashcard = MapToDto(flashcard)
            };
        }

        // Updates a flashcard if the user owns it.
        public async Task<FlashcardResult> UpdateAsync(
            int flashcardId, UpdateFlashcardDto dto, string userId)
        {
            // The repository returns false for both a missing flashcard and a database
            // error, so this check is what separates "not found" from "failed".
            var existing = await _flashcardRepository.GetByIdAsync(flashcardId, userId);

            if (existing == null)
            {
                return new FlashcardResult { Status = FlashcardResultStatus.NotFound };
            }

            existing.Front = dto.Front.Trim();
            existing.Back = dto.Back.Trim();

            var updated = await _flashcardRepository.UpdateAsync(existing, userId);

            return new FlashcardResult
            {
                Status = updated ? FlashcardResultStatus.Success : FlashcardResultStatus.Failed
            };
        }

        // Deletes a flashcard if the user owns it.
        public async Task<FlashcardResult> DeleteAsync(int flashcardId, string userId)
        {
            var existing = await _flashcardRepository.GetByIdAsync(flashcardId, userId);

            if (existing == null)
            {
                return new FlashcardResult { Status = FlashcardResultStatus.NotFound };
            }

            var deleted = await _flashcardRepository.DeleteAsync(flashcardId, userId);

            return new FlashcardResult
            {
                Status = deleted ? FlashcardResultStatus.Success : FlashcardResultStatus.Failed
            };
        }

        // Copies only the fields the client is allowed to see.
        private static FlashcardDto MapToDto(Flashcard flashcard)
        {
            return new FlashcardDto
            {
                FlashcardId = flashcard.FlashcardId,
                Front = flashcard.Front,
                Back = flashcard.Back
            };
        }
    }
}
