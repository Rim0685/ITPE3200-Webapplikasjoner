// The Services folder contains the application's business logic.
// This class enforces the deck rules and maps entities to DTOs.
// All database access goes through the repository.

using QuizApi.DAL;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Services
{
    public class FlashcardDeckService : IFlashcardDeckService
    {
        // The maximum number of decks a single user can own.
        public const int MaxDecksPerUser = 10;

        private readonly IFlashcardDeckRepository _deckRepository;
        private readonly ILogger<FlashcardDeckService> _logger;

        public FlashcardDeckService(
            IFlashcardDeckRepository deckRepository,
            ILogger<FlashcardDeckService> logger)
        {
            _deckRepository = deckRepository;
            _logger = logger;
        }

        // Retrieves all decks, without the flashcards inside them.
        public async Task<IEnumerable<FlashcardDeckDto>> GetAllAsync(string userId)
        {
            var decks = await _deckRepository.GetAllAsync(userId);

            return decks.Select(d => MapToDto(d, includeCards: false));
        }

        // Retrieves a deck with its flashcards, or null if the user does not own it.
        public async Task<FlashcardDeckDto?> GetByIdAsync(int deckId, string userId)
        {
            var deck = await _deckRepository.GetByIdAsync(deckId, userId);

            return deck == null ? null : MapToDto(deck, includeCards: true);
        }

        // Creates a deck unless the limit is reached or the name is already used.
        public async Task<FlashcardDeckResult> CreateAsync(
            CreateFlashcardDeckDto dto, string userId)
        {
            var name = dto.Name.Trim();

            var count = await _deckRepository.CountAsync(userId);

            if (count >= MaxDecksPerUser)
            {
                _logger.LogWarning(
                    "[FlashcardDeckService] User {UserId} has reached the deck limit.",
                    userId);

                return new FlashcardDeckResult { Status = FlashcardDeckResultStatus.LimitReached };
            }

            if (await _deckRepository.NameExistsAsync(userId, name))
            {
                return new FlashcardDeckResult { Status = FlashcardDeckResultStatus.DuplicateName };
            }

            // The owner comes from the caller's token, never from the client's data.
            var deck = new FlashcardDeck
            {
                Name = name,
                Description = CleanDescription(dto.Description),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _deckRepository.CreateAsync(deck);

            if (!created)
            {
                return new FlashcardDeckResult { Status = FlashcardDeckResultStatus.Failed };
            }

            return new FlashcardDeckResult
            {
                Status = FlashcardDeckResultStatus.Success,
                Deck = MapToDto(deck, includeCards: false)
            };
        }

        // Updates a deck if the user owns it and the new name is not used by another deck.
        public async Task<FlashcardDeckResult> UpdateAsync(
            int deckId, UpdateFlashcardDeckDto dto, string userId)
        {
            var existing = await _deckRepository.GetByIdAsync(deckId, userId);

            if (existing == null)
            {
                return new FlashcardDeckResult { Status = FlashcardDeckResultStatus.NotFound };
            }

            var name = dto.Name.Trim();

            if (await _deckRepository.NameExistsAsync(userId, name, deckId))
            {
                return new FlashcardDeckResult { Status = FlashcardDeckResultStatus.DuplicateName };
            }

            existing.Name = name;
            existing.Description = CleanDescription(dto.Description);

            var updated = await _deckRepository.UpdateAsync(existing, userId);

            return new FlashcardDeckResult
            {
                Status = updated ? FlashcardDeckResultStatus.Success : FlashcardDeckResultStatus.Failed
            };
        }

        // Deletes a deck (and its flashcards) if the user owns it.
        public async Task<FlashcardDeckResult> DeleteAsync(int deckId, string userId)
        {
            var existing = await _deckRepository.GetByIdAsync(deckId, userId);

            if (existing == null)
            {
                return new FlashcardDeckResult { Status = FlashcardDeckResultStatus.NotFound };
            }

            var deleted = await _deckRepository.DeleteAsync(deckId, userId);

            return new FlashcardDeckResult
            {
                Status = deleted ? FlashcardDeckResultStatus.Success : FlashcardDeckResultStatus.Failed
            };
        }

        // An empty description is stored as null.
        private static string? CleanDescription(string? description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        // Copies only the fields the client is allowed to see.
        private static FlashcardDeckDto MapToDto(FlashcardDeck deck, bool includeCards)
        {
            return new FlashcardDeckDto
            {
                FlashcardDeckId = deck.FlashcardDeckId,
                Name = deck.Name,
                Description = deck.Description,
                CreatedAt = deck.CreatedAt,
                CardCount = deck.Flashcards.Count,
                Flashcards = includeCards
                    ? deck.Flashcards
                        .OrderBy(f => f.FlashcardId)
                        .Select(f => new FlashcardDto
                        {
                            FlashcardId = f.FlashcardId,
                            Front = f.Front,
                            Back = f.Back,
                            DeckId = f.DeckId
                        })
                        .ToList()
                    : new List<FlashcardDto>()
            };
        }
    }
}