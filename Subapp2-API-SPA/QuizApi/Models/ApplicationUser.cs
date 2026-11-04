// The Models folder contains the application's domain entities.
// This class represents a registered user of the application.

using Microsoft.AspNetCore.Identity;

namespace QuizApi.Models
{
    // IdentityUser already provides Id, UserName, Email and PasswordHash.
    public class ApplicationUser : IdentityUser
    {
        // Contains the personal flashcards owned by this user.
        public List<Flashcard> Flashcards { get; set; }
            = new List<Flashcard>();

        // Contains the quiz attempts completed by this user.
        public List<QuizAttempt> QuizAttempts { get; set; }
            = new List<QuizAttempt>();
    }
}
