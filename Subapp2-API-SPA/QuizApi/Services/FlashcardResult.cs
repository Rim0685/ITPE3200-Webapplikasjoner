// The Services folder contains the application's business logic.
// These types describe the outcome of a flashcard operation, so the
// controller can choose the correct HTTP response.

using QuizApi.DTOs;

namespace QuizApi.Services
{
    public enum FlashcardResultStatus
    {
        Success,
        NotFound,
        LimitReached,
        InvalidDeck,
        Failed
    }

    public class FlashcardResult
    {
        public FlashcardResultStatus Status { get; set; }

        // Contains the flashcard when an operation creates one.
        public FlashcardDto? Flashcard { get; set; }
    }
}
