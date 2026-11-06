// The DTOs folder contains the data sent between the API and the client.
// This class is the flashcard returned to the client.
// UserId is left out because the client has no need for it.

namespace QuizApi.DTOs
{
    public class FlashcardDto
    {
        public int FlashcardId { get; set; }

        public string Front { get; set; } = string.Empty;

        public string Back { get; set; } = string.Empty;
    }
}
