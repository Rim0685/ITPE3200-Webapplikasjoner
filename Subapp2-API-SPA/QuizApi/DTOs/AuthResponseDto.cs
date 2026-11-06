// The DTOs folder contains the data sent between the API and the client.
// This class is returned after a successful registration or login.

namespace QuizApi.DTOs
{
    public class AuthResponseDto
    {
        // The client sends this token in the Authorization header on later requests.
        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // The time (UTC) when the token stops being valid.
        public DateTime ExpiresAt { get; set; }
    }
}
