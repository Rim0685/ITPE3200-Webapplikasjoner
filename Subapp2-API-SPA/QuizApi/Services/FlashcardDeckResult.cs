// The Services folder contains the application's business logic.
// These types describe the outcome of a deck operation, so the
// controller can choose the correct HTTP response.

using QuizApi.DTOs;

namespace QuizApi.Services
{
    public enum FlashcardDeckResultStatus
    {
        Success,
        NotFound,
        LimitReached,
        DuplicateName,
        Failed
    }

    public class FlashcardDeckResult
    {
        public FlashcardDeckResultStatus Status { get; set; }

        // Contains the deck when an operation creates one.
        public FlashcardDeckDto? Deck { get; set; }
    }
}