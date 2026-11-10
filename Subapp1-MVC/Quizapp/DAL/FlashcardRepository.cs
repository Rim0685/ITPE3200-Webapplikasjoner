using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    public class FlashcardRepository : IFlashcardRepository
    {
        private readonly QuizDbContext _db;

        public FlashcardRepository(QuizDbContext db)
        {
            _db = db;
        }

        // Retrieves the user's flashcards, including their deck.
        public async Task<IEnumerable<Flashcard>> GetAll(string userId)
        {
            return await _db.Flashcards
                .AsNoTracking()
                .Include(card => card.Deck)
                .Where(card => card.UserId == userId)
                .OrderByDescending(card => card.CreatedAt)
                .ToListAsync();
        }

        // Retrieves a flashcard belonging to the user.
        public async Task<Flashcard?> GetById(int id, string userId)
        {
            return await _db.Flashcards
                .FirstOrDefaultAsync(card =>
                    card.FlashcardId == id
                    && card.UserId == userId);
        }

        public async Task Create(Flashcard flashcard)
        {
            _db.Flashcards.Add(flashcard);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Update(
            Flashcard flashcard, string userId)
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