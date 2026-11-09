// Implements database operations for a user's flashcards.

using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    public class FlashcardRepository : IFlashcardRepository
    {
        private readonly QuizDbContext _db;

        // Receives the database context through dependency injection.
        public FlashcardRepository(QuizDbContext db)
        {
            _db = db;
        }

        // Retrieves the user's flashcards, with the newest first.
        public async Task<IEnumerable<Flashcard>> GetAll(string userId)
        {
            return await _db.Flashcards
                .AsNoTracking()
                .Where(card => card.UserId == userId)
                .OrderByDescending(card => card.CreatedAt)
                .ToListAsync();
        }

        // Retrieves a flashcard only if it belongs to the user.
        public async Task<Flashcard?> GetById(int id, string userId)
        {
            return await _db.Flashcards
                .FirstOrDefaultAsync(card =>
                    card.FlashcardId == id
                    && card.UserId == userId);
        }

        // Saves a new flashcard.
        public async Task Create(Flashcard flashcard)
        {
            _db.Flashcards.Add(flashcard);
            await _db.SaveChangesAsync();
        }

        // Updates only the editable fields of the user's flashcard.
        public async Task<bool> Update(
            Flashcard flashcard,
            string userId)
        {
            var existingCard = await _db.Flashcards
                .FirstOrDefaultAsync(card =>
                    card.FlashcardId == flashcard.FlashcardId
                    && card.UserId == userId);

            if (existingCard == null)
            {
                return false;
            }

            existingCard.QuestionText = flashcard.QuestionText;
            existingCard.AnswerText = flashcard.AnswerText;

            await _db.SaveChangesAsync();

            return true;
        }

        // Deletes a flashcard only if it belongs to the user.
        public async Task<bool> Delete(int id, string userId)
        {
            var flashcard = await _db.Flashcards
                .FirstOrDefaultAsync(card =>
                    card.FlashcardId == id
                    && card.UserId == userId);

            if (flashcard == null)
            {
                return false;
            }

            _db.Flashcards.Remove(flashcard);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}